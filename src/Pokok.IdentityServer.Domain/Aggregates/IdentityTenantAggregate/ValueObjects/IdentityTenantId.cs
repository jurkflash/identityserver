using Pokok.BuildingBlocks.Domain.Abstractions;

namespace Pokok.IdentityServer.Domain.Aggregates.IdentityTenantAggregate.ValueObjects
{
    public sealed class IdentityTenantId : ValueObject
    {
        public Guid Value { get; }

        private IdentityTenantId(Guid value) => Value = value;

        public static IdentityTenantId New() => new(Guid.NewGuid());

        public static IdentityTenantId FromGuid(Guid value) => new(value);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
