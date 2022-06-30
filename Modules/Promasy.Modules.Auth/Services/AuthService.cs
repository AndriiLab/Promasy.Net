using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Persistence;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Core.Auth;

namespace Promasy.Modules.Auth.Services;

internal class AuthService : IAuthService
{
    private readonly IDatabase _database;

    public AuthService(IDatabase database)
    {
        _database = database;
    }

    public async Task<int?> AuthAsync(string userName, string password)
    {
        var userData = await _database.Employees
            .AsNoTracking()
            .Where(e => e.UserName == userName)
            .Select(e => new
            {
                e.Id,
                PasswordHash = e.Password,
                PasswordSalt = e.Salt
            })
            .FirstOrDefaultAsync();

        if (userData is null)
        {
            return null;
        }

        if (!PasswordHelper.Validate(password, userData.PasswordHash, userData.PasswordSalt))
        {
            return null;
        }

        // migrate to new password validation scheme
        if (userData.PasswordSalt is not null)
        {
            await ChangePasswordAsync(userData.Id, password);
        }

        return userData.Id;
    }

    public async Task ChangePasswordAsync(int userId, string newPassword)
    {
        var hash = PasswordHelper.Hash(newPassword);
        var user = await _database.Employees.FirstOrDefaultAsync(e => e.Id == userId);
        if (user is null)
        {
            return;
        }

        user.Password = hash;
        user.Salt = null;
        await _database.SaveChangesAsync();
    }
}