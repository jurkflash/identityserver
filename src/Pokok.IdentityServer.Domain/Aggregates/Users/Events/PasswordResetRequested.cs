using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Pokok.IdentityServer.Domain.ValueObjects;

namespace Pokok.IdentityServer.Domain.Aggregates.Users.Events
{
    public sealed class PasswordResetRequested : IDomainEvent
    {
        public UserId UserId { get; }
        public Email Email { get; }
        public DisplayName DisplayName { get; }
        public ConfirmationLink ResetLink { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public PasswordResetRequested(UserId userId, Email email, DisplayName displayName, ConfirmationLink resetLink)
        {
            UserId = userId;
            Email = email;
            DisplayName = displayName;
            ResetLink = resetLink;
        }
    }
}
