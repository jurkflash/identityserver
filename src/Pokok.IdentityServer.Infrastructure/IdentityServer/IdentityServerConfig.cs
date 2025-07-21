using Duende.IdentityServer.Models;

namespace Pokok.IdentityServer.Infrastructure.IdentityServer
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<Client> GetClients(string environment)
        {
            var clients = new List<Client>();

            // Shared clients for all environments
            clients.Add(new Client
            {
                ClientId = "pokok-portal",
                ClientName = "Pokok JMB Portal",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequireClientSecret = false,
                AllowedScopes = { "openid", "profile", "pokok.api" },
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600
            });

            if (environment == "Development")
            {
                clients.Add(new Client
                {
                    ClientId = "swagger-dev",
                    ClientName = "Swagger (Development)",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("dev-secret".Sha256()) },
                    AllowedScopes = { "pokok.api" }
                });
            }

            if (environment == "Production")
            {
                clients.Add(new Client
                {
                    ClientId = "external-client-prod",
                    ClientName = "External Client (Prod)",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("prod-secret".Sha256()) },
                    AllowedScopes = { "pokok.api" }
                });
            }

            return clients;
        }

    public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    };

    public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
    {
        new ApiScope("pokok.api", "Pokok API")
    };

    public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
    {
        new ApiResource("pokok.api", "Pokok API")
        {
            Scopes = { "pokok.api" }
        }
    };
    }
}
