using Microsoft.EntityFrameworkCore;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Domain.Entities;
using Pokok.IdentityServer.Infrastructure.Identity;

namespace Pokok.IdentityServer.Infrastructure.Persistence;

public class TenantStore : ITenantStore
{
    private readonly IdentityDbContext _context;

    public TenantStore(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
    }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain && t.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsValidAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AnyAsync(t => t.Id == tenantId && t.IsActive, cancellationToken);
    }

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
