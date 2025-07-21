using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Events
{
    public sealed class UserRegisteredDomainEvent : IDomainEvent
    {
        public UserId UserId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserRegisteredDomainEvent(UserId userId)
        {
            UserId = userId;
        }
    }
}
