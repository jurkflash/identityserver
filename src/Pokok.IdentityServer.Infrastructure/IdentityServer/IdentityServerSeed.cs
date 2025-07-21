using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Pokok.IdentityServer.Infrastructure.IdentityServer
{
    public static class IdentityServerSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, string environment)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                foreach (var client in IdentityServerConfig.GetClients(environment))
                    context.Clients.Add(client.ToEntity());

                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityServerConfig.GetIdentityResources())
                    context.IdentityResources.Add(resource.ToEntity());

                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var scopeObj in IdentityServerConfig.GetApiScopes())
                    context.ApiScopes.Add(scopeObj.ToEntity());

                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var api in IdentityServerConfig.GetApiResources())
                    context.ApiResources.Add(api.ToEntity());

                await context.SaveChangesAsync();
            }
        }
    }
}
