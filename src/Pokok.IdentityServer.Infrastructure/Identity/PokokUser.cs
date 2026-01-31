using Microsoft.AspNetCore.Identity;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class PokokUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        public string? TenantId { get; set; }

        private PokokUser() { } // EF

        public PokokUser(string? tenantId = null)
        {
            TenantId = tenantId;
        }
    }
}
