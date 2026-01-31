using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Pokok.IdentityServer.Infrastructure.Identity;
using System.Security.Claims;

namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer;

public class PokokProfileService : IProfileService
{
    private readonly UserManager<PokokUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<PokokUser> _claimsFactory;

    public PokokProfileService(
        UserManager<PokokUser> userManager,
        IUserClaimsPrincipalFactory<PokokUser> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user == null)
        {
            return;
        }

        var principal = await _claimsFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();

        // Add tenant_id to issued claims
        if (!string.IsNullOrWhiteSpace(user.TenantId))
        {
            if (!claims.Any(c => c.Type == "tenant_id"))
            {
                claims.Add(new Claim("tenant_id", user.TenantId));
            }
        }

        // Add display name
        if (!string.IsNullOrWhiteSpace(user.DisplayName))
        {
            if (!claims.Any(c => c.Type == "display_name"))
            {
                claims.Add(new Claim("display_name", user.DisplayName));
            }
        }

        // Filter by requested claim types
        context.IssuedClaims = claims
            .Where(c => context.RequestedClaimTypes.Contains(c.Type))
            .ToList();

        // Always include tenant_id even if not explicitly requested
        if (!context.IssuedClaims.Any(c => c.Type == "tenant_id") && !string.IsNullOrWhiteSpace(user.TenantId))
        {
            context.IssuedClaims.Add(new Claim("tenant_id", user.TenantId));
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
