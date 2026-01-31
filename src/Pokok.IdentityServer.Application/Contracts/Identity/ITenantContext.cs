namespace Pokok.IdentityServer.Application.Contracts.Identity;

/// <summary>
/// Holds the current tenant context for the request
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant identifier
    /// </summary>
    string? TenantId { get; }

    /// <summary>
    /// Gets whether a tenant is currently resolved
    /// </summary>
    bool HasTenant { get; }

    /// <summary>
    /// Sets the current tenant
    /// </summary>
    void SetTenant(string tenantId);

    /// <summary>
    /// Clears the current tenant context
    /// </summary>
    void Clear();
}
