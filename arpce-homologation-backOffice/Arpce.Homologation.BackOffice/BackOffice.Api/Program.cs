// Fichier : BackOffice.Api/Program.cs

using BackOffice.Api.Middleware;
using BackOffice.Application;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Infrastructure.Persistence;
using BackOffice.Infrastructure.Security;
using BackOffice.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System; // Pour ArgumentNullException

// --- Configuration initiale de Serilog (logger de démarrage) ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Démarrage du microservice BackOffice API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // --- Configuration de Serilog ---
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // --- Configuration des Services ---

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // Configuration Swagger avec authentification JWT
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ARPCE Homologation - BackOffice API",
            Version = "v1",
            Description = "API pour la gestion des demandes d'homologation interne de ARPCE."
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Veuillez entrer 'Bearer' suivi d'un espace et du token JWT. Exemple: 'Bearer VOTRE_TOKEN'",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // Configuration CORS
    var corsPolicyName = "AllowWebApp";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicyName,
                          policy =>
                          {
                              policy.AllowAnyOrigin() // À restreindre en production
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
    });

    // Configuration DB Context
    builder.Services.AddDbContext<BackOfficeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(BackOfficeDbContext).Assembly.FullName)));

    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<BackOfficeDbContext>());

    // Configuration MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // Configuration Authentification JWT
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Lecture et validation robuste des secrets pour le déploiement
            var jwtSecret = builder.Configuration["JwtSettings:Secret"];
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = builder.Configuration["JwtSettings:Audience"];

            Log.Information("BackOffice - Configuration JWT - Secret: {Secret}, Issuer: {Issuer}, Audience: {Audience}",
                            !string.IsNullOrWhiteSpace(jwtSecret) ? "CONFIGURED (length " + jwtSecret.Length + ")" : "NOT CONFIGURED",
                            jwtIssuer ?? "NOT CONFIGURED",
                            jwtAudience ?? "NOT CONFIGURED");

            // Lève une exception claire si le secret est manquant sur Azure
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtSecret, "Le secret JWT (JwtSettings:Secret) est manquant ou vide.");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

    // Injection des services
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    // --- Construction du Pipeline HTTP ---

    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    // --- Activation conditionnelle de Swagger ---
    // Active Swagger si on est en Dev OU si le paramètre EnableSwaggerUI est à true (pour le Sandbox)
    bool enableSwagger = app.Environment.IsDevelopment() ||
                         builder.Configuration.GetValue<bool>("EnableSwaggerUI", false);

    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API V1");
            options.RoutePrefix = string.Empty; // Swagger à la racine du site

            // Injection du CSS personnalisé pour le thème BackOffice
            options.InjectStylesheet("/css/swagger-custom.css");
        });
    }

    // --- Application automatique des migrations au démarrage (pour Sandbox/Dev) ---
    bool applyMigrationsOnStartup = app.Environment.IsDevelopment() ||
                                    app.Environment.EnvironmentName == "Staging" ||
                                    builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);

    if (applyMigrationsOnStartup)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<BackOfficeDbContext>();
                context.Database.Migrate();
                Log.Information("Migrations EF Core appliquées avec succès au BackOffice.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur critique lors de l'application des migrations EF Core au BackOffice.");
            }
        }
    }

    app.UseHttpsRedirection();

    // ESSENTIEL : Permet de servir le fichier CSS personnalisé depuis wwwroot
    app.UseStaticFiles();

    app.UseCors(corsPolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application BackOffice s'est arrêtée de manière inattendue.");
}
finally
{
    Log.CloseAndFlush();
}