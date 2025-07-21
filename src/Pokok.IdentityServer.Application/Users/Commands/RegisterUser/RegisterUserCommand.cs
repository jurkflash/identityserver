using Pokok.BuildingBlocks.Cqrs.Abstractions;

namespace Pokok.IdentityServer.Application.Users.Commands.RegisterUser
{
    public sealed class RegisterUserCommand : ICommand<Guid>
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string? DisplayName { get; init; }
    }
}
