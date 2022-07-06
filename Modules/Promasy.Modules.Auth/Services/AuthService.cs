using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Promasy.Core.UserContext;
using Promasy.Domain.Persistence;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Core.Auth;
using Promasy.Modules.Core.Extensions;
using Promasy.Modules.Core.UserContext;

namespace Promasy.Modules.Auth.Services;

internal class AuthService : IAuthService
{
    private readonly IDatabase _database;
    private readonly IUserContextResolver _userContextResolver;
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IDatabase database, IUserContextResolver userContextResolver,
        IEmployeesRepository employeesRepository, IHttpContextAccessor httpContextAccessor)
    {
        _database = database;
        _userContextResolver = userContextResolver;
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
        var employeeDto = await _employeesRepository.GetEmployeeByIdAsync(id);
        if (employeeDto is null)
        {
            return;
        }

        var context = new UserContext(
            employeeDto.Id,
            employeeDto.FirstName,
            employeeDto.MiddleName,
            employeeDto.LastName,
            employeeDto.Email,
            employeeDto.Organization,
            employeeDto.OrganizationId,
            employeeDto.Department,
            employeeDto.DepartmentId,
            employeeDto.SubDepartment,
            employeeDto.SubDepartmentId,
            _httpContextAccessor.HttpContext?.GetIpAddress(),
            employeeDto.Roles);
        _userContextResolver.Set(context);
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