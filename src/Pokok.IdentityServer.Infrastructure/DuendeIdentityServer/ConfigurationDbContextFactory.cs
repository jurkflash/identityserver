using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer
{
    public class ConfigurationDbContextFactory :
            IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext(string[] args)
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

            // Step 2: Set up options
            var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            optionsBuilder.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("Pokok.IdentityServer.Infrastructure"));

            var storeOptions = new ConfigurationStoreOptions
            {
                // Optional: define your schema name
                // DefaultSchema = "your_schema_name"
            };

            // Fix: Use the constructor that accepts only DbContextOptions
            var context = new ConfigurationDbContext(optionsBuilder.Options);
            context.StoreOptions = storeOptions; // Assign store options separately

            return context;
        }
    }
}
