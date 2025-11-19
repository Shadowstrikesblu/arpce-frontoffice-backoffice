<<<<<<< HEAD
=======
// Fichier : FrontOffice.Api/Program.cs

// --- Directives 'using' ---
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
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
<<<<<<< HEAD

// --- Configuration initiale de Serilog ---
=======
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Ajouté pour GetValue<bool> plus loin
using System; // Ajouté pour ArgumentNullException.ThrowIfNullOrWhiteSpace

// --- Configuration initiale de Serilog (logger de démarrage) ---
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

<<<<<<< HEAD
Log.Information("Starting up the FrontOffice API");
=======
Log.Information("Démarrage du microservice FrontOffice API...");
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

<<<<<<< HEAD
    // --- Configuration des Services ---

    // Accesseur au contexte HTTP (nï¿½cessaire pour ICurrentUserService)
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();
    // Enregistrement du service pour l'utilisateur courant
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    // Politique CORS
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

    // Base de donnï¿½es avec SQL Server
    builder.Services.AddDbContext<FrontOfficeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<FrontOfficeDbContext>());

    // Contrï¿½leurs
    builder.Services.AddControllers();

    // Documentation API (Swagger UI avec support JWT)
=======
    // --- Configuration des Services (Injection de Dépendances) ---

    builder.Services.AddControllers();
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ARPCE Homologation - FrontOffice API",
            Version = "v1",
<<<<<<< HEAD
            Description = "API pour la gestion des demandes d'homologation cï¿½tï¿½ client."
        });

        // Configuration pour la sï¿½curitï¿½ JWT dans Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Veuillez entrer 'Bearer' suivi d'un espace et du token JWT",
=======
            Description = "API pour la gestion des demandes d'homologation côté client."
        });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Veuillez entrer 'Bearer' suivi d'un espace et du token JWT. Exemple: 'Bearer VOTRE_TOKEN'",
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
<<<<<<< HEAD
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

    // MediatR (pour CQRS)
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // Configuration de l'Authentification JWT
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
=======
        options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });
    });

    var corsPolicyName = "AllowWebApp";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicyName,
                          policy =>
                          {
                              policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                          });
    });

    builder.Services.AddDbContext<FrontOfficeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<FrontOfficeDbContext>());

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // --- Configuration de l'Authentification JWT ---
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // --- NOUVEAU : Lecture robuste et validation des paramètres JWT ---
            var jwtSecret = builder.Configuration["JwtSettings:Secret"];
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = builder.Configuration["JwtSettings:Audience"];

            // AJOUT DU LOG ET VALIDATION : Pour le débogage sur Azure, cela va nous dire explicitement si c'est null.
            Log.Information("Configuration JWT - Secret: {Secret}, Issuer: {Issuer}, Audience: {Audience}",
                            jwtSecret != null ? "CONFIGURED (length " + jwtSecret.Length + ")" : "NOT CONFIGURED",
                            jwtIssuer ?? "NOT CONFIGURED",
                            jwtAudience ?? "NOT CONFIGURED");

            // Lève une ArgumentNullException plus tôt et plus clairement si le secret est manquant.
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtSecret, nameof(jwtSecret));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtIssuer, nameof(jwtIssuer));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtAudience, nameof(jwtAudience));

>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
<<<<<<< HEAD
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
            };
        });

    // Injection des services personnalisï¿½s
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();


    // --- Pipeline de Requï¿½tes HTTP ---

    var app = builder.Build();

    // Le middleware de gestion d'erreurs doit ï¿½tre l'un des premiers
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Middleware Serilog pour logger les requï¿½tes HTTP
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
=======

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                // Utilise maintenant le secret validé
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();


    // --- Construction de l'application et du Pipeline de Requêtes HTTP ---

    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    bool enableSwagger = app.Environment.IsDevelopment() ||
                         builder.Configuration.GetValue<bool>("EnableSwaggerUI", false);

    if (enableSwagger)
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FrontOffice API V1");
            options.RoutePrefix = string.Empty;
<<<<<<< HEAD
            options.InjectStylesheet("/css/swagger-custom.css");
        });
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseCors(corsPolicyName);

    // Activer l'authentification avant l'autorisation
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://*:{port}");
=======
        });
    }

    // --- Configuration du démarrage de la base de données (Migrations) ---
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
                var context = services.GetRequiredService<FrontOfficeDbContext>();
                context.Database.Migrate();
                Log.Information("Migrations EF Core appliquées avec succès au FrontOffice.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'application des migrations EF Core au FrontOffice.");
            }
        }
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors(corsPolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a

    app.Run();
}
catch (Exception ex)
{
<<<<<<< HEAD
    Log.Fatal(ex, "Application terminated unexpectedly");
=======
    Log.Fatal(ex, "L'application FrontOffice s'est arrêtée de manière inattendue.");
>>>>>>> ba50e5adcea7f9534963efefde2fb3c819d57c3a
}
finally
{
    Log.CloseAndFlush();
}