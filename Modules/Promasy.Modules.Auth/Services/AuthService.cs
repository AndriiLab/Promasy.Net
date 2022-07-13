using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Core.Auth;

namespace Promasy.Modules.Auth.Services;

internal class AuthService : IAuthService
{
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IEmployeesRepository employeesRepository, IHttpContextAccessor httpContextAccessor)
    {
        _employeesRepository = employeesRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<int?> AuthAsync(string userName, string password)
    {
        var userData = await _employeesRepository.GetEmployeeDataByUserNameAsync(userName);
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
            await SetEmployeePasswordAsync(userData.Id, password);
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

    public Task SetEmployeePasswordAsync(int id, string password)
    {
        return _employeesRepository.SetEmployeePasswordAsync(id, PasswordHelper.Hash(password));
    }
}