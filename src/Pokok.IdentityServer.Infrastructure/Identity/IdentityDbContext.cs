using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pokok.IdentityServer.Application.Contracts.Identity;
using Pokok.IdentityServer.Domain.Entities;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class IdentityDbContext : IdentityDbContext<PokokUser, PokokRole, string>
    {
        private readonly ITenantContext? _tenantContext;

        public DbSet<Tenant> Tenants => Set<Tenant>();

        public IdentityDbContext(
            DbContextOptions<IdentityDbContext> options,
            ITenantContext? tenantContext = null)
            : base(options)
        {
            _tenantContext = tenantContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureTenant(builder);
            ConfigurePokokUser(builder);
        }

        private void ConfigurePokokUser(ModelBuilder builder)
        {
            builder.Entity<PokokUser>(entity =>
            {
                entity.Property(u => u.DisplayName)
                      .HasMaxLength(200);

                entity.Property(u => u.TenantId)
                      .HasMaxLength(450);

                entity.HasIndex(u => u.TenantId);

                // Global query filter for tenant isolation
                // Rules:
                // 1. System-level users (TenantId == null) are always visible (e.g., system admins)
                // 2. When no tenant context is set, all users are visible (for login flow)
                // 3. When tenant context is set, only users in that tenant (+ system users) are visible
                if (_tenantContext != null)
                {
                    entity.HasQueryFilter(u => 
                        u.TenantId == null ||  // System-level users always visible
                        !_tenantContext.HasTenant ||  // No tenant = show all (login flow)
                        u.TenantId == _tenantContext.TenantId);  // Tenant-specific users
                }
            });
        }

        private static void ConfigureTenant(ModelBuilder builder)
        {
            builder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Subdomain)
                      .HasMaxLength(100);

                entity.HasIndex(t => t.Subdomain)
                      .IsUnique();

                entity.Property(t => t.IsActive)
                      .IsRequired();

                entity.Property(t => t.CreatedAt)
                      .IsRequired();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically set TenantId on new entities (only for non-system users)
            if (_tenantContext?.HasTenant == true)
            {
                foreach (var entry in ChangeTracker.Entries<PokokUser>())
                {
                    if (entry.State == EntityState.Added && string.IsNullOrWhiteSpace(entry.Entity.TenantId))
                    {
                        entry.Entity.TenantId = _tenantContext.TenantId;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
