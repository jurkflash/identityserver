using Duende.IdentityServer.Models;

namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<Client> GetClients(IEnumerable<ClientConfiguration> clientConfigs)
        {
            var clients = new List<Client>();

            foreach (var config in clientConfigs)
            {
                var client = new Client
                {
                    ClientId = config.ClientId,
                    ClientName = config.ClientName,
                    AllowedGrantTypes = MapGrantTypes(config.GrantTypes),
                    RequireClientSecret = config.RequireClientSecret,
                    AllowedScopes = config.AllowedScopes,
                    AllowOfflineAccess = config.AllowOfflineAccess,
                    AccessTokenLifetime = config.AccessTokenLifetime
                };

                if (config.ClientSecrets.Any())
                {
                    client.ClientSecrets = config.ClientSecrets
                        .Select(secret => new Secret(secret.Sha256()))
                        .ToList();
                }

                clients.Add(client);
            }

            return clients;
        }

        private static ICollection<string> MapGrantTypes(List<string> grantTypes)
        {
            var mapped = new List<string>();
            foreach (var grantType in grantTypes)
            {
                mapped.Add(grantType.ToLowerInvariant() switch
                {
                    "password" => GrantType.ResourceOwnerPassword,
                    "client_credentials" => GrantType.ClientCredentials,
                    "authorization_code" => GrantType.AuthorizationCode,
                    "implicit" => GrantType.Implicit,
                    "hybrid" => GrantType.Hybrid,
                    _ => grantType
                });
            }
            return mapped;
        }

        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),

            // Custom identity scope for tenant awareness
            new IdentityResource(
                name: "pokok.identity",
                displayName: "Pokok Identity",
                userClaims: new[] { "tenant_id" }
            )
        };

        public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
        {
            // Pokok Living product
            new ApiScope("pokok.living.api", "Pokok Living API"),

            // Identity management (admin / internal)
            new ApiScope("pokok.identity.admin", "Pokok Identity Admin"),

            // Internal service-to-service
            new ApiScope("pokok.internal.user.write", "Internal User Write Access"),
            new ApiScope("pokok.internal.user.read", "Internal User Read Access")
        };

        public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
        {
            new ApiResource("pokok.living", "Pokok Living Backend")
            {
                Scopes =
                {
                    "pokok.living.api"
                }
            },

            new ApiResource("pokok.identity", "Pokok Identity API")
            {
                Scopes =
                {
                    "pokok.identity.admin",
                    "pokok.internal.user.write",
                    "pokok.internal.user.read"
                }
            }
        };
    }
}
