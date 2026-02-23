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

// Ajoute les contr�leurs
builder.Services.AddControllers();

// Ajout du Service SignalR (N�cessaire pour les notifications temps r�el)
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

    // Configuration pour la s�curit� JWT dans Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
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
            Array.Empty<string>()
        }
    });
});

// Configuration de la base de donn�es
builder.Services.AddDbContext<BackOfficeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(BackOfficeDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<BackOfficeDbContext>());

// Ajouter MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Ajoute les services pour la s�curit� et l'utilisateur courant
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ILdapService, LdapService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IFileStorageProvider, DatabaseFileStorageProvider>();
builder.Services.AddTransient<INotificationService, SignalRNotificationService>();
builder.Services.AddTransient<ICertificateGeneratorService, CertificateGeneratorService>();
builder.Services.AddTransient<IDevisGeneratorService, DevisGeneratorService>();
builder.Services.AddTransient<IReceiptGeneratorService, ReceiptGeneratorService>();

// Configuration de l'Authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };

        // --- Configuration sp�cifique pour SignalR ---
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // Si la requ�te a un token ET qu'elle cible le Hub SignalR
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/notifications"))
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

// ------------------------------------------------------
// BUILD
// ------------------------------------------------------
var app = builder.Build();

// --- Configuration du Pipeline HTTP ---

// Utiliser le middleware de gestion d'erreurs
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API V1");
        options.RoutePrefix = string.Empty; // Swagger � la racine

        // INJECTION DU CSS PERSONNALIS�
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

// Mapping du Hub SignalR � l'URL "/hubs/notifications"
app.MapHub<NotificationHub>("/hubs/notifications");

// Optional: if you want "/" to load index.html (SPA), DO NOT MapGet("/").
// Use index.html in wwwroot. If you want a health endpoint:
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();
