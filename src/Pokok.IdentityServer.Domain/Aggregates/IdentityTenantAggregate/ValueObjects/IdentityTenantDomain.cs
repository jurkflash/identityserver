using Pokok.BuildingBlocks.Domain.Abstractions;
using System.Text.RegularExpressions;

namespace Pokok.IdentityServer.Domain.Aggregates.IdentityTenantAggregate.ValueObjects
{
    public sealed class IdentityTenantDomain : ValueObject
    {
        public string Value { get; }

        private IdentityTenantDomain(string value) => Value = value;

        public static IdentityTenantDomain Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Domain cannot be empty.", nameof(value));

            var normalized = value.Trim().ToLowerInvariant();

            if (!Regex.IsMatch(normalized, @"^[a-z0-9.-]+\.[a-z]{2,}$"))
                throw new ArgumentException("Invalid domain format.", nameof(value));

            return new IdentityTenantDomain(normalized);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
