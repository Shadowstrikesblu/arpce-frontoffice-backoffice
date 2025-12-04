using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FrontOffice.Infrastructure.Persistence;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove SQL Server DB
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<FrontOfficeDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add InMemory DB
            services.AddDbContext<FrontOfficeDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Override JWT options if needed
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateLifetime = false;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                }
            );
        });
    }
}
