using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
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
            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            optionsBuilder.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("Pokok.IdentityServer.Infrastructure"));

            var storeOptions = new OperationalStoreOptions
            {
                // Optional: define your schema name
                // DefaultSchema = "your_schema_name"
            };

            var context = new PersistedGrantDbContext(optionsBuilder.Options);
            context.StoreOptions = storeOptions; // Assign store options separately

            return context;
        }
    }
}
