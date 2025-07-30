using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Pokok.IdentityServer.Domain.Aggregates.Users.Events;
using Pokok.IdentityServer.Domain.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.Users
{
    public sealed class User : AggregateRoot<UserId>
    {
        public UserId UserId { get; private set; }
        public Email Email { get; private set; }
        public DisplayName DisplayName { get; private set; }
        public ConfirmationLink ConfirmationLink { get; private set; }

        private User(UserId userId, Email email, DisplayName displayName, ConfirmationLink confirmationLink)
            : base(userId)
        {
            Email = email;
            UserId = userId;
            DisplayName = displayName;
            ConfirmationLink = confirmationLink;

            AddDomainEvent(new UserRegistrationConfirmationRequestedDomainEvent(userId, email, displayName, confirmationLink));
        }

        public static User Register(UserId userId, Email email, DisplayName displayName, ConfirmationLink confirmationLink)
        {
            return new User(userId, email, displayName, confirmationLink);
        }
    }
}