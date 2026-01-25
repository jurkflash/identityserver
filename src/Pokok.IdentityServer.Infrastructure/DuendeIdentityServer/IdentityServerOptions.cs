namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer
{
    public class IdentityServerOptions
    {
        public const string SectionName = "IdentityServer";

        public List<ClientConfiguration> Clients { get; set; } = new();
    }

    public class ClientConfiguration
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public List<string> GrantTypes { get; set; } = new();
        public bool RequireClientSecret { get; set; } = true;
        public List<string> ClientSecrets { get; set; } = new();
        public List<string> AllowedScopes { get; set; } = new();
        public bool AllowOfflineAccess { get; set; }
        public int AccessTokenLifetime { get; set; } = 3600;
    }
}
