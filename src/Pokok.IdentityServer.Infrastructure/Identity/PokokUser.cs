using Microsoft.AspNetCore.Identity;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class PokokUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        public int IdentityTenantId { get; private set; }

        private PokokUser() { } // EF

        public PokokUser(int identityTenantId)
        {
            IdentityTenantId = identityTenantId;
        }
    }
}
