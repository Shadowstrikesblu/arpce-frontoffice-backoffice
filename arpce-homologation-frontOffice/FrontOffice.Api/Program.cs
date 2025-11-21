// Fichier : FrontOffice.Api/Program.cs

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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Ajout� pour GetValue<bool> plus loin
using System; // Ajout� pour ArgumentNullException.ThrowIfNullOrWhiteSpace

// --- Configuration initiale de Serilog (logger de d�marrage) ---
=======

>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

<<<<<<< HEAD
Log.Information("D�marrage du microservice FrontOffice API...");
=======
Log.Information("Démarrage du microservice FrontOffice API...");
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

<<<<<<< HEAD
    // --- Configuration des Services (Injection de D�pendances) ---

=======
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ARPCE Homologation - FrontOffice API",
            Version = "v1",
<<<<<<< HEAD
            Description = "API pour la gestion des demandes d'homologation c�t� client."
=======
            Description = "API pour la gestion des demandes d'homologation côté client."
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Veuillez entrer 'Bearer' suivi d'un espace et du token JWT.",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] {} }
        });
    });

    var corsPolicyName = "AllowWebApp";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicyName, policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

    // -----------------------
    // DATABASE CONFIGURATION
    // -----------------------
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        builder.Services.AddDbContext<FrontOfficeDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<FrontOfficeDbContext>());

        Log.Information("🔗 Database enabled (connection string length {Length})", connectionString.Length);
    }
    else
    {
        Log.Warning("⚠ No database connection string found — running without DB.");
    }

    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // -----------------------
    // JWT AUTHENTICATION
    // -----------------------
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
<<<<<<< HEAD
            // --- NOUVEAU : Lecture robuste et validation des param�tres JWT ---
=======
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
            var jwtSecret = builder.Configuration["JwtSettings:Secret"];
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = builder.Configuration["JwtSettings:Audience"];

<<<<<<< HEAD
            // AJOUT DU LOG ET VALIDATION : Pour le d�bogage sur Azure, cela va nous dire explicitement si c'est null.
=======
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
            Log.Information("Configuration JWT - Secret: {Secret}, Issuer: {Issuer}, Audience: {Audience}",
                            jwtSecret != null ? "CONFIGURED" : "NOT CONFIGURED",
                            jwtIssuer ?? "NOT CONFIGURED",
                            jwtAudience ?? "NOT CONFIGURED");

<<<<<<< HEAD
            // L�ve une ArgumentNullException plus t�t et plus clairement si le secret est manquant.
=======
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtSecret, nameof(jwtSecret));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtIssuer, nameof(jwtIssuer));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(jwtAudience, nameof(jwtAudience));

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
<<<<<<< HEAD
                // Utilise maintenant le secret valid�
=======
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();

<<<<<<< HEAD

    // --- Construction de l'application et du Pipeline de Requ�tes HTTP ---
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5000);
    });
=======
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    // Enable Swagger conditionally
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

        Log.Information("Swagger enabled.");
    }
    else
    {
        Log.Information("Swagger disabled.");
    }

<<<<<<< HEAD
    // --- Configuration du d�marrage de la base de donn�es (Migrations) ---
    bool applyMigrationsOnStartup = app.Environment.IsDevelopment() ||
                                    app.Environment.EnvironmentName == "Staging" ||
=======
    // -----------------------
    // SAFE EF MIGRATIONS
    // -----------------------
    bool applyMigrationsOnStartup = !string.IsNullOrWhiteSpace(connectionString) &&
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
                                    builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);

    if (applyMigrationsOnStartup)
    {
        try
        {
<<<<<<< HEAD
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<FrontOfficeDbContext>();
                context.Database.Migrate();
                Log.Information("Migrations EF Core appliqu�es avec succ�s au FrontOffice.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'application des migrations EF Core au FrontOffice.");
            }
=======
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FrontOfficeDbContext>();
            context.Database.Migrate();
            Log.Information("EF Core migrations applied successfully.");
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
        }
        catch (Exception ex)
        {
            Log.Error(ex, "EF migration failed — continuing without DB.");
        }
    }
    else
    {
        Log.Warning("EF migrations skipped.");
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors(corsPolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
<<<<<<< HEAD
    Log.Fatal(ex, "L'application FrontOffice s'est arr�t�e de mani�re inattendue.");
=======
    Log.Fatal(ex, "L'application FrontOffice s'est arrêtée de manière inattendue.");
>>>>>>> 4b5b167010a3556e8d77ae5c4146198c8d8167a2
}
finally
{
    Log.CloseAndFlush();
}
