using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Events;

namespace Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Aggregates
{
    public sealed class User : AggregateRoot<UserId>
    {
        public string Email { get; private set; }
        public string HashedPassword { get; private set; }
        public string? DisplayName { get; private set; }

        // Required by EF Core
        private User() { }

        private User(UserId id, string email, string hashedPassword, string? displayName = null)
            : base(id)
        {
            Email = email;
            HashedPassword = hashedPassword;
            DisplayName = displayName;

            //AddDomainEvent(new UserRegisteredDomainEvent(Id));
        }

        public static User Register(string email, string hashedPassword, string? displayName = null)
        {
            var id = new UserId(Guid.NewGuid());
            return new User(id, email, hashedPassword, displayName);
        }

        public void ChangeDisplayName(string newName)
        {
            DisplayName = newName;
            // You can raise another domain event if needed.
        }
    }
}
