using FrontOffice.Api.Middleware;
using FrontOffice.Application;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Infrastructure.Persistence;
using FrontOffice.Infrastructure.Security;
using FrontOffice.Infrastructure.Services;
using FrontOffice.Infrastructure.SignalR;
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

Log.Information("Starting up the FrontOffice API");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    // --- Configuration des Services ---

    // Accesseur au contexte HTTP (nécessaire pour ICurrentUserService)
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IFileStorageProvider, DatabaseFileStorageProvider>();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddHttpClient<ICaptchaValidator, GoogleCaptchaValidator>();
    builder.Services.AddHttpClient<MtnPaymentService>();
    builder.Services.AddHttpClient<AirtelPaymentService>();
    builder.Services.AddSignalR();
    builder.Services.AddTransient<INotificationService, SignalRNotificationService>();

    // Enregistre tous les IPaymentService dans le conteneur
    builder.Services.AddTransient<IPaymentService, MtnPaymentService>();
    builder.Services.AddTransient<IPaymentService, AirtelPaymentService>();

    // Crée une "factory" simple pour les récupérer par leur code
    builder.Services.AddTransient<Func<string, IPaymentService>>(serviceProvider => providerCode =>
    {
        var services = serviceProvider.GetServices<IPaymentService>();
        return services.FirstOrDefault(s => s.ProviderCode.Equals(providerCode, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Service de paiement '{providerCode}' non supporté.");
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

    // Base de données avec SQL Server
    builder.Services.AddDbContext<FrontOfficeDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApplicationDbContext>(provider =>
        provider.GetRequiredService<FrontOfficeDbContext>());

    // Contrôleurs
    builder.Services.AddControllers();

    // Documentation API (Swagger UI avec support JWT)
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ARPCE Homologation - FrontOffice API",
            Version = "v1",
            Description = "API pour la gestion des demandes d'homologation côté client."
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

    // MediatR (pour CQRS)
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

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

    // Injection des services personnalisés
    builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

    builder.Services.AddTransient<IEmailService, EmailService>();


    // --- Pipeline de Requêtes HTTP ---

    var app = builder.Build();

    // Le middleware de gestion d'erreurs doit être l'un des premiers
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Middleware Serilog pour logger les requêtes HTTP
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FrontOffice API V1");
            options.RoutePrefix = string.Empty;
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

    app.MapHub<NotificationHub>("/hubs/notifications");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}