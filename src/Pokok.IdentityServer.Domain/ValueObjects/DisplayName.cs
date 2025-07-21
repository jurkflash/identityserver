using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.IdentityServer.Domain.ValueObjects
{
    public sealed class DisplayName : ValueObject
    {
        public string Value { get; }

        private DisplayName(string value)
        {
            Value = value;
        }

        public static DisplayName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Display name cannot be empty.", nameof(value));

            return new DisplayName(value.Trim());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
