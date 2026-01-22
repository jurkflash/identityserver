using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pokok.IdentityServer.Domain.Entities;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class IdentityDbContext : IdentityDbContext<PokokUser, PokokRole, string>
    {
        public DbSet<IdentityTenant> IdentityTenants => Set<IdentityTenant>();

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureIdentityTenant(builder);
        }

        private static void ConfigureIdentityTenant(ModelBuilder builder)
        {
            builder.Entity<PokokUser>(entity =>
            {
                entity.Property(u => u.DisplayName)
                      .HasMaxLength(200);

                entity.Property(u => u.IdentityTenantId)
                      .IsRequired();
            });

            builder.Entity<IdentityTenant>(entity =>
            {
                entity.ToTable("IdentityTenants");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasIndex(t => t.Name)
                      .IsUnique();

                entity.Property(t => t.Description)
                      .HasMaxLength(500);

                entity.Property(t => t.IsActive)
                      .IsRequired();
            });
        }
    }
}
