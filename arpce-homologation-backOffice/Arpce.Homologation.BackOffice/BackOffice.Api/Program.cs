using BackOffice.Api.Middleware;
using BackOffice.Application;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Infrastructure.Persistence;
using BackOffice.Infrastructure.Security;
using BackOffice.Infrastructure.Services;
using BackOffice.Infrastructure.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// --- Configuration initiale de Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Démarrage du microservice BackOffice API sur le VPS...");

// Logique de Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Ajoute les contr�leurs
builder.Services.AddControllers();

// Ajout du Service SignalR (N�cessaire pour les notifications temps r�el)
builder.Services.AddSignalR();

// Configuration Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var builder = WebApplication.CreateBuilder(args);

    // --- Configuration de Serilog ---
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // ------------------------------------------------------
    //              SERVICES
    // ------------------------------------------------------

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    // CORS
    var corsPolicyName = "AllowWebApp";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(corsPolicyName, policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

    // DbContext
    builder.Services.AddDbContext<BackOfficeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(BackOfficeDbContext).Assembly.FullName)));

    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<BackOfficeDbContext>());

    builder.Services.AddControllers();

    // 🔥 Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ARPCE Homologation - BackOffice API",
            Version = "v1",
            Description = "API pour la gestion interne des demandes d'homologation."
        });

        // Fix Swagger duplicate schema names
        options.CustomSchemaIds(type => type.FullName);

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Entrez : Bearer <token>",
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
                Array.Empty<string>()
            }
        });
    });

    // MediatR
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // Security services (ADD ONCE!)
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddTransient<ILdapService, LdapService>();
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddScoped<IFileStorageProvider, DatabaseFileStorageProvider>();

// Ajouter MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Ajoute les services pour la s�curit� et l'utilisateur courant
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ILdapService, LdapService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IFileStorageProvider, DatabaseFileStorageProvider>();
builder.Services.AddTransient<INotificationService, SignalRNotificationService>();
builder.Services.AddTransient<ICertificateGeneratorService, CertificateGeneratorService>();

// Configuration de l'Authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };

        // --- Configuration sp�cifique pour SignalR ---
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // Si la requ�te a un token ET qu'elle cible le Hub SignalR
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                {
                    // On injecte le token manuellement dans le contexte pour que l'authentification fonctionne
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });


// Politique CORS
// Attention : Pour SignalR avec Authentification, AllowAnyOrigin() n'est pas permis.
// Il faut utiliser WithOrigins(...) et AllowCredentials().
var corsPolicyName = "AllowWebApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
                      policy =>
                      {
                          policy.AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials() 
                                .SetIsOriginAllowed(origin => true); // Autorise toutes les origines tout en permettant AllowCredentials
                      });
});

var app = builder.Build();

// --- Configuration du Pipeline HTTP ---

// Utiliser le middleware de gestion d'erreurs
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.ListenAnyIP(4000);
    });

    //-----------------------------------------------------
    //         BUILD
    //-----------------------------------------------------

    var app = builder.Build();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    // Enable Swagger
    bool enableSwagger = app.Environment.IsDevelopment() ||
                         builder.Configuration.GetValue<bool>("EnableSwaggerUI", false);

    if (enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API V1");
            options.RoutePrefix = string.Empty;
            options.InjectStylesheet("/css/swagger-custom.css");
        });

        Log.Information("Swagger UI activé sur BackOffice.");
    }

    // Migrations auto
    bool applyMigrations = builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);
    if (applyMigrations)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erreur lors de l'application des migrations.");
        }
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
    Log.Fatal(ex, "L'application BackOffice s'est arrêtée de manière inattendue sur le VPS.");
}
finally
{
    Log.CloseAndFlush();
}

app.UseHttpsRedirection();

// Permet de servir les fichiers statiques (comme le fichier CSS dans wwwroot)
app.UseStaticFiles();

app.UseCors(corsPolicyName);

// Activer l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Mapping du Hub SignalR � l'URL "/hubs/notifications"
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
