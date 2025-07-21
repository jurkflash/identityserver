using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.IdentityServer.Domain.Aggregates.IdentityTenantAggregate.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.IdentityTenantAggregate.Aggregates
{
    public class IdentityTenant : AggregateRoot<IdentityTenantId>
    {
        public string Name { get; private set; }
        public IdentityTenantDomain? Domain { get; private set; }
        public bool IsEnabled { get; private set; }

        private readonly List<Guid> _userIds = new();
        public IReadOnlyCollection<Guid> UserIds => _userIds.AsReadOnly();

        private IdentityTenant() { } // EF Core

        private IdentityTenant(IdentityTenantId id, string name, IdentityTenantDomain? domain)
        {
            Id = id;
            Name = name;
            Domain = domain;
            IsEnabled = true;
        }

        public static IdentityTenant Create(string name, IdentityTenantDomain? domain = null)
        {
            return new IdentityTenant(IdentityTenantId.New(), name, domain);
        }

        public void Disable() => IsEnabled = false;

        public void AddUser(Guid userId)
        {
            if (!_userIds.Contains(userId))
                _userIds.Add(userId);
        }

        public void RemoveUser(Guid userId)
        {
            _userIds.Remove(userId);
        }
    }
}
