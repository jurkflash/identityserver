using Pokok.IdentityServer.Application.Contracts.Identity;

namespace Pokok.IdentityServer.Infrastructure.Identity;

/// <summary>
/// Scoped service that holds the current tenant context
/// </summary>
public class TenantContext : ITenantContext
{
    private string? _tenantId;

    public string? TenantId => _tenantId;

    public bool HasTenant => !string.IsNullOrWhiteSpace(_tenantId);

    public void SetTenant(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("Tenant ID cannot be null or empty", nameof(tenantId));
        }

        _tenantId = tenantId;
    }

    public void Clear()
    {
        _tenantId = null;
    }
}
