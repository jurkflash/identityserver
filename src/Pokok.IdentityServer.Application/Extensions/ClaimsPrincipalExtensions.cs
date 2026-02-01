using System.Security.Claims;

namespace Pokok.IdentityServer.Application.Extensions
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal to extract common claims
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public const string TenantIdClaimType = "tenant_id";

        /// <summary>
        /// Gets the tenant_id claim from the principal.
        /// This is typically set from client credentials for API clients.
        /// </summary>
        public static string? GetTenantId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(TenantIdClaimType);
        }

        /// <summary>
        /// Gets the tenant_id claim or throws if not present.
        /// </summary>
        public static string GetRequiredTenantId(this ClaimsPrincipal principal)
        {
            var tenantId = principal.GetTenantId();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new UnauthorizedAccessException(
                    "Tenant ID claim is required but was not found in the access token. " +
                    "Ensure the client is configured with a tenant_id.");
            }
            return tenantId;
        }

        /// <summary>
        /// Gets the client_id claim from the principal.
        /// </summary>
        public static string? GetClientId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("client_id");
        }
    }
}
