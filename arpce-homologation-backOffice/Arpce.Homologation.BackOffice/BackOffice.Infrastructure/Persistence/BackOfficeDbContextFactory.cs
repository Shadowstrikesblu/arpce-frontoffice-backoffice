using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BackOffice.Infrastructure.Persistence
{
    public class BackOfficeDbContextFactory
        : IDesignTimeDbContextFactory<BackOfficeDbContext>
    {
        public BackOfficeDbContext CreateDbContext(string[] args)
        {
            // 🔥 Force base path to API project
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "BackOffice.Api");

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BackOfficeDbContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            return new BackOfficeDbContext(optionsBuilder.Options);
        }
    }
}
