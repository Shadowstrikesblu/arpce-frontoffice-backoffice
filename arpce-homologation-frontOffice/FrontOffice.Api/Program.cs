using FrontOffice.Api.Middleware;
using FrontOffice.Api.Services;
using FrontOffice.Application;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Infrastructure.Persistence;
using FrontOffice.Infrastructure.Security;
using FrontOffice.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// --- Configuration initiale de Serilog ---
// Permet de logger les erreurs survenant avant le démarrage complet de l'app
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Démarrage du microservice FrontOffice API sur le VPS...");

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

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddHttpClient<ICaptchaValidator, GoogleCaptchaValidator>();

    // Politique CORS (Ouverte pour le Sandbox)
    var corsPolicyName = "AllowWebApp";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicyName,
                          policy =>
                          {
                              policy.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
    });

    // Base de données SQL Server
    builder.Services.AddDbContext<FrontOfficeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<FrontOfficeDbContext>());

    builder.Services.AddControllers();

    // Configuration Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ARPCE Homologation - FrontOffice API",
            Version = "v1",
            Description = "API pour la gestion des demandes d'homologation côté client."
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Veuillez entrer 'Bearer' suivi d'un espace et du token JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] {}
            }
        });
    });

    // MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // --- Configuration Authentification JWT Robuste ---
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtSecret = builder.Configuration["JwtSettings:Secret"];
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = builder.Configuration["JwtSettings:Audience"];

            // Logs pour aider l'infra à déboguer (sans afficher le secret en clair)
            Log.Information("FrontOffice - Config JWT - Secret: {SecretStatus}, Issuer: {Issuer}, Audience: {Audience}",
                !string.IsNullOrWhiteSpace(jwtSecret) ? "PRESENT (Long: " + jwtSecret.Length + ")" : "MANQUANT",
                jwtIssuer,
                jwtAudience);

            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                throw new ArgumentNullException("JwtSettings:Secret", "Le secret JWT est manquant dans la configuration.");
            }

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

    // Injection des services personnalisés
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddTransient<IEmailService, EmailService>();

    // --- Construction de l'application ---

    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    // --- Activation de Swagger (Logique Sandbox) ---
    // Active Swagger si on est en Dev OU si le paramètre EnableSwaggerUI est à true dans appsettings.json
    bool enableSwagger = app.Environment.IsDevelopment() ||
                         builder.Configuration.GetValue<bool>("EnableSwaggerUI", false);

    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FrontOffice API V1");
            options.RoutePrefix = string.Empty;
            options.InjectStylesheet("/css/swagger-custom.css");
        });
        Log.Information("Swagger UI activé sur FrontOffice.");
    }

    // --- Migration Automatique ---
    bool applyMigrations = builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);
    
    if (applyMigrations)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<FrontOfficeDbContext>();
                context.Database.Migrate();
                Log.Information("Migrations EF Core appliquées avec succès au FrontOffice.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur critique lors de l'application des migrations sur le VPS.");
            }
        }
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles(); // Indispensable pour le CSS Swagger et les uploads
    app.UseCors(corsPolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "L'application FrontOffice s'est arrêtée de manière inattendue sur le VPS.");
}
finally
{
    Log.CloseAndFlush();
}