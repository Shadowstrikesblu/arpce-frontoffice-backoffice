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

var builder = WebApplication.CreateBuilder(args);

// --- Configuration des Services ---

// Logique de Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Ajoute les contrôleurs
builder.Services.AddControllers();

// Ajout du Service SignalR (Nécessaire pour les notifications temps réel)
builder.Services.AddSignalR();

// Configuration Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ARPCE Homologation - BackOffice API",
        Version = "v1",
        Description = "API pour la gestion des demandes d'homologation interne de ARPCE."
    });

    // Configuration pour la sécurité JWT dans Swagger
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

// Configuration de la base de données
builder.Services.AddDbContext<BackOfficeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(BackOfficeDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<BackOfficeDbContext>());

// Ajouter MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Ajoute les services pour la sécurité et l'utilisateur courant
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
builder.Services.AddTransient<IDevisGeneratorService, DevisGeneratorService>();

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

        // --- Configuration spécifique pour SignalR ---
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // Si la requête a un token ET qu'elle cible le Hub SignalR
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
// On utilisera WithOrigins(...) et AllowCredentials().
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API V1");
        options.RoutePrefix = string.Empty; // Swagger à la racine

        // INJECTION DU CSS PERSONNALISÉ
        options.InjectStylesheet("/css/swagger-custom.css");
    });
}

app.UseHttpsRedirection();

// Permet de servir les fichiers statiques (comme le fichier CSS dans wwwroot)
app.UseStaticFiles();

app.UseCors(corsPolicyName);

// Activer l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Mapping du Hub SignalR à l'URL "/hubs/notifications"
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();