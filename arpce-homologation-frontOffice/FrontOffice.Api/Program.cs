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

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Démarrage du microservice FrontOffice API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddControllers();
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
            var jwtSecret = builder.Configuration["JwtSettings:Secret"];
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
            var jwtAudience = builder.Configuration["JwtSettings:Audience"];

            Log.Information("Configuration JWT - Secret: {Secret}, Issuer: {Issuer}, Audience: {Audience}",
                            jwtSecret != null ? "CONFIGURED" : "NOT CONFIGURED",
                            jwtIssuer ?? "NOT CONFIGURED",
                            jwtAudience ?? "NOT CONFIGURED");

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IFileStorageProvider, LocalFileStorageProvider>();

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
        });

        Log.Information("Swagger enabled.");
    }
    else
    {
        Log.Information("Swagger disabled.");
    }

    // -----------------------
    // SAFE EF MIGRATIONS
    // -----------------------
    bool applyMigrationsOnStartup = !string.IsNullOrWhiteSpace(connectionString) &&
                                    builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);

    if (applyMigrationsOnStartup)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FrontOfficeDbContext>();
            context.Database.Migrate();
            Log.Information("EF Core migrations applied successfully.");
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
    Log.Fatal(ex, "L'application FrontOffice s'est arrêtée de manière inattendue.");
}
finally
{
    Log.CloseAndFlush();
}
