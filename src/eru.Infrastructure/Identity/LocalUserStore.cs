using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eru.Infrastructure.Identity
{
    public class LocalUserStore : IUserPasswordStore<User>, IQueryableUserStore<User>, IUserLockoutStore<User>
    {
        private readonly UserDbContext _userDb;

        public LocalUserStore(UserDbContext userDb)
        {
            _userDb = userDb;
        }

        public void Dispose()
        {
            _userDb.Dispose();
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _userDb.AddAsync(user, cancellationToken);
            await _userDb.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            _userDb.Remove(user);
            await _userDb.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userDb.Users.FindAsync(userId, cancellationToken);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _userDb.Users.FirstOrDefaultAsync(x => x.NormalizedUsername == normalizedUserName, cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUsername);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUsername = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _userDb.Update(user);
            await _userDb.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public IQueryable<User> Users => _userDb.Users;
        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailed);
        }

        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEndDate);
        }

        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(++user.AccessFailed);
        }

        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailed = 0;
            return Task.CompletedTask;
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEndDate = lockoutEnd;
            return Task.CompletedTask;
        }
    }
}