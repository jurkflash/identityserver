using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.IdentityServer.Domain.ValueObjects
{
    public class ConfirmationLink : SingleValueObject<string>
    {
        public ConfirmationLink(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Confirmation link cannot be null or empty.", nameof(value));
            }

            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                throw new ArgumentException("Invalid confirmation link URL format.", nameof(value));
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
