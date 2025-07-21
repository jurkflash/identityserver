using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Repositories;

namespace Pokok.IdentityServer.Application.Users.Commands.RegisterUser
{
    //public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly IPasswordHasher _passwordHasher;

    //    public RegisterUserCommandHandler(
    //        IUserRepository userRepository,
    //        IPasswordHasher passwordHasher)
    //    {
    //        _userRepository = userRepository;
    //        _passwordHasher = passwordHasher;
    //    }

    //    public async Task<Guid> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    //    {
    //        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
    //        if (existingUser is not null)
    //            throw new InvalidOperationException("User with this email already exists.");

    //        var hashedPassword = _passwordHasher.HashPassword(command.Password);
    //        var user = User.Register(command.Email, hashedPassword, command.DisplayName);

    //        await _userRepository.AddAsync(user, cancellationToken);

    //        return user.Id.Value;
    //    }
    //}
}
