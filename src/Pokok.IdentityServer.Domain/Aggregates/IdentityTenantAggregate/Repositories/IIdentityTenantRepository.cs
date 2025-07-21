using Pokok.IdentityServer.Domain.Aggregates.IdentityTenantAggregate.Aggregates;
using Pokok.IdentityServer.Domain.Aggregates.IdentityTenantAggregate.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.TenantAggregate.Repositories
{
    public interface IIdentityTenantRepository
    {
        Task<IdentityTenant?> GetByIdAsync(IdentityTenantId id, CancellationToken cancellationToken = default);
        Task<IdentityTenant?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
        Task AddAsync(IdentityTenant tenant, CancellationToken cancellationToken = default);
    }
}
