using Microsoft.AspNetCore.Identity;
using Pokok.IdentityServer.Domain.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Entities
{
    public class User : IdentityUser<Guid>
    {
        public DisplayName? DisplayName { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Guid? TenantId { get; private set; }

        public void SetDisplayName(DisplayName displayName)
        {
            DisplayName = displayName;
        }

        public void Deactivate() => IsActive = false;
    }
}
