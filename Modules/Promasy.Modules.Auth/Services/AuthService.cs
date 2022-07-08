using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Persistence;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Core.Auth;

namespace Promasy.Modules.Auth.Services;

internal class AuthService : IAuthService
{
    private readonly IDatabase _database;
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IDatabase database,
        IEmployeesRepository employeesRepository, IHttpContextAccessor httpContextAccessor)
    {
        _database = database;
        _employeesRepository = employeesRepository;
        _httpContextAccessor = httpContextAccessor;
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
                PasswordSalt = e.Salt,
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

        await SetUserContextAsync(userData.Id);
    
        // migrate to new password validation scheme
        if (userData.PasswordSalt is not null)
        {
            await ChangePasswordAsync(userData.Id, password);
        }

        return userData.Id;
    }

    public async Task SetUserContextAsync(int id)
    {
        var claims = await _employeesRepository.GetEmployeeClaimsByIdAsync(id);
        if (claims is null)
        {
            return;
        }

        if (_httpContextAccessor.HttpContext is not null)
        {
            _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
        }
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