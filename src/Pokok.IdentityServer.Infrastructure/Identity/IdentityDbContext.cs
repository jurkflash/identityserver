using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Outbox;
using System.Text.Json;

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
        }
    }
}
