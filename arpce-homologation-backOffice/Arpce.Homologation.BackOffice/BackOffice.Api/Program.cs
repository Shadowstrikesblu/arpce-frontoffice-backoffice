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

// ------------------------------------------------------
// SERILOG
// ------------------------------------------------------
builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

// ------------------------------------------------------
// SERVICES
// ------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

// ------------------------------------------------------
// SWAGGER (needed for UseSwagger/UseSwaggerUI)
// ------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackOffice API", Version = "v1" });

    // JWT auth support in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// DbContext
builder.Services.AddDbContext<BackOfficeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(BackOfficeDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<BackOfficeDbContext>());

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Security & infrastructure
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

// ------------------------------------------------------
// AUTH (JWT) + SignalR support
// ------------------------------------------------------
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

        // SignalR JWT support
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

// ------------------------------------------------------
// CORS (SignalR + Auth requires AllowCredentials, so no AllowAnyOrigin)
// ------------------------------------------------------
var corsPolicyName = "AllowWebApp";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName, policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true); // allow all origins with credentials
        // Recommended for prod:
        // .WithOrigins("https://your-frontend-domain.com")
    });
});

// ------------------------------------------------------
// BUILD
// ------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------
// PIPELINE
// ------------------------------------------------------
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSerilogRequestLogging();

// Swagger enabled in Production via config
bool enableSwagger =
    app.Environment.IsDevelopment() ||
    app.Configuration.GetValue<bool>("EnableSwaggerUI");

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API V1");
        options.RoutePrefix = "swagger";

        // INJECTION DU CSS PERSONNALIS�
        options.InjectStylesheet("/css/swagger-custom.css");
    });
}

// Optional root endpoint (avoids 404 on /)
app.MapGet("/", () => Results.Ok("BackOffice API is running"));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS must be between UseRouting and UseAuthentication/UseAuthorization
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();
