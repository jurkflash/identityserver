using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer
{
    public static class IdentityServerSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var identityServerOptions = scope.ServiceProvider.GetRequiredService<IOptions<IdentityServerOptions>>().Value;
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("IdentityServerSeed");

            await SeedClientsAsync(context, identityServerOptions, logger);
            await SeedIdentityResourcesAsync(context, logger);
            await SeedApiScopesAsync(context, logger);
            await SeedApiResourcesAsync(context, logger);
        }

        private static async Task SeedClientsAsync(ConfigurationDbContext context, IdentityServerOptions options, ILogger logger)
        {
            var existingClientIds = await context.Clients
                .Select(c => c.ClientId)
                .ToListAsync();

            foreach (var client in IdentityServerConfig.GetClients(options.Clients))
            {
                if (!existingClientIds.Contains(client.ClientId))
                {
                    context.Clients.Add(client.ToEntity());
                    logger.LogInformation("Client '{ClientId}' seeded", client.ClientId);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedIdentityResourcesAsync(ConfigurationDbContext context, ILogger logger)
        {
            var existingResourceNames = await context.IdentityResources
                .Select(r => r.Name)
                .ToListAsync();

            foreach (var resource in IdentityServerConfig.GetIdentityResources())
            {
                if (!existingResourceNames.Contains(resource.Name))
                {
                    context.IdentityResources.Add(resource.ToEntity());
                    logger.LogInformation("Identity resource '{Name}' seeded", resource.Name);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedApiScopesAsync(ConfigurationDbContext context, ILogger logger)
        {
            var existingScopeNames = await context.ApiScopes
                .Select(s => s.Name)
                .ToListAsync();

            foreach (var apiScope in IdentityServerConfig.GetApiScopes())
            {
                if (!existingScopeNames.Contains(apiScope.Name))
                {
                    context.ApiScopes.Add(apiScope.ToEntity());
                    logger.LogInformation("API scope '{Name}' seeded", apiScope.Name);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedApiResourcesAsync(ConfigurationDbContext context, ILogger logger)
        {
            var existingResourceNames = await context.ApiResources
                .Select(r => r.Name)
                .ToListAsync();

            foreach (var resource in IdentityServerConfig.GetApiResources())
            {
                if (!existingResourceNames.Contains(resource.Name))
                {
                    context.ApiResources.Add(resource.ToEntity());
                    logger.LogInformation("API resource '{Name}' seeded", resource.Name);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
