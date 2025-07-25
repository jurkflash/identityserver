﻿using Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Entities;

namespace Pokok.IdentityServer.Domain.Aggregates.UserAggregate.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
    }
}
