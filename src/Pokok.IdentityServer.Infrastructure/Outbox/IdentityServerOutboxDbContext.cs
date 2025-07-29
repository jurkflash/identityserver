using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Outbox;

namespace Pokok.IdentityServer.Infrastructure.Outbox
{
    public class IdentityServerOutboxDbContext : OutboxDbContext
    {
        public IdentityServerOutboxDbContext(DbContextOptions<IdentityServerOutboxDbContext> options)
            : base(options)
        {
        }
    }
}
