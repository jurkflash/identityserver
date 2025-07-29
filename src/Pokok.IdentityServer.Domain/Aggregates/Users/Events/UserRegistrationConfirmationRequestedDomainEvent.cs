using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Pokok.IdentityServer.Domain.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.Users.Events
{
    public sealed class UserRegistrationConfirmationRequestedDomainEvent : IDomainEvent
    {
        public UserId UserId { get; }
        public Email Email { get; }
        public DisplayName DisplayName { get; }
        public ConfirmationLink ConfirmationLink { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public UserRegistrationConfirmationRequestedDomainEvent(UserId userId, Email email, DisplayName displayName, ConfirmationLink confirmationLink)
        {
            UserId = userId;
            Email = email;
            DisplayName = displayName;
            ConfirmationLink = confirmationLink;
        }
    }
}
