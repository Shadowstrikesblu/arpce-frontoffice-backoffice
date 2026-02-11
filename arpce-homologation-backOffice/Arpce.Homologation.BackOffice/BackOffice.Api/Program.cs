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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BackOffice API", Version = "v1" });

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

        // SignalR JWT support via query string
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

builder.Services.AddAuthorization();

// ------------------------------------------------------
// CORS
// ------------------------------------------------------
var corsPolicyName = "AllowWebApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true);
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

// Swagger (you can keep always on if you want)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BackOffice API V1");
    options.RoutePrefix = "swagger";
    options.InjectStylesheet("/css/swagger-custom.css");
});

app.UseRouting();

// Serve index.html from wwwroot if present (SPA / static site)
app.UseDefaultFiles();  // looks for index.html by default
app.UseStaticFiles();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Optional: if you want "/" to load index.html (SPA), DO NOT MapGet("/").
// Use index.html in wwwroot. If you want a health endpoint:
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();
