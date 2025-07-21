using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Pokok.IdentityServer.Infrastructure.Identity
{
    public class IdentityDbContext : IdentityDbContext<PokokUser, PokokRole, string>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize table names if desired
            //builder.Entity<PokokUser>().ToTable("Users");
            //builder.Entity<PokokRole>().ToTable("Roles");
            //builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            //builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            //builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            //builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            //builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        }
    }
}
