using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.Extensions.DependencyInjection;
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

            if (!context.Clients.Any())
            {
                foreach (var client in IdentityServerConfig.GetClients(identityServerOptions.Clients))
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
