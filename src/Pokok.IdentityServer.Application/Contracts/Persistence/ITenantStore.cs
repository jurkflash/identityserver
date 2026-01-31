using Pokok.IdentityServer.Domain.Entities;

namespace Pokok.IdentityServer.Application.Contracts.Persistence;

/// <summary>
/// Store for tenant operations
/// </summary>
public interface ITenantStore
{
    /// <summary>
    /// Gets a tenant by identifier
    /// </summary>
    Task<Tenant?> GetByIdAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tenant by subdomain
    /// </summary>
    Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active tenants
    /// </summary>
    Task<IEnumerable<Tenant>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a tenant exists and is active
    /// </summary>
    Task<bool> IsValidAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new tenant
    /// </summary>
    Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tenant
    /// </summary>
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
}
