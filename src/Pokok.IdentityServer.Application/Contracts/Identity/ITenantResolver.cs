namespace Pokok.IdentityServer.Application.Contracts.Identity;

/// <summary>
/// Resolves tenant from the current request context
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// Resolves the tenant identifier from the current request
    /// </summary>
    /// <returns>Tenant identifier if found, otherwise null</returns>
    Task<string?> ResolveAsync();
}
