using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pokok.BuildingBlocks.Outbox;

namespace Pokok.IdentityServer.Infrastructure.Outbox
{
    public class IdentityServerOutboxDbContextFactory : IDesignTimeDbContextFactory<IdentityServerOutboxDbContext>
    {
        public IdentityServerOutboxDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../Pokok.IdentityServer.Presentation"));
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("IdentityConnection");

            var optionsBuilder = new DbContextOptionsBuilder<OutboxDbContext>();
            optionsBuilder.UseNpgsql(connectionString, b =>
            {
                b.MigrationsAssembly("Pokok.IdentityServer.Infrastructure");
            });
            return new IdentityServerOutboxDbContext(optionsBuilder.Options);
        }
    }
}
