using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Pokok.IdentityServer.Infrastructure.Identity;

public class PokokUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<PokokUser, PokokRole>
{
    public PokokUserClaimsPrincipalFactory(
        UserManager<PokokUser> userManager,
        RoleManager<PokokRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(PokokUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Add tenant claim
        if (!string.IsNullOrWhiteSpace(user.TenantId))
        {
            identity.AddClaim(new Claim("tenant_id", user.TenantId));
        }

        // Add display name if available
        if (!string.IsNullOrWhiteSpace(user.DisplayName))
        {
            identity.AddClaim(new Claim("display_name", user.DisplayName));
        }

        return identity;
    }
}
