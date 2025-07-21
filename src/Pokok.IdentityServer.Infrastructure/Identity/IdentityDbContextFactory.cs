using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            // Step 1: Load configuration from appsettings.Development.json
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Pokok.IdentityServer.Presentation"));
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("IdentityConnection");
            Console.WriteLine("Base path: " + AppContext.BaseDirectory);
            Console.WriteLine("Connection: " + config.GetConnectionString("IdentityConnection"));
            // Step 2: Set up options
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("Pokok.IdentityServer.Infrastructure"));

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}
