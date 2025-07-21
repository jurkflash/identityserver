using Microsoft.AspNetCore.Identity;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class PokokUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        // Optional: For global multi-tenancy
        public Guid? TenantId { get; set; }
    }
}
