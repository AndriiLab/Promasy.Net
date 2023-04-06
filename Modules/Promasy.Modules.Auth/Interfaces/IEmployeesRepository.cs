using System.Security.Claims;
using Promasy.Application.Interfaces;
using Promasy.Modules.Auth.Dtos;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Auth.Interfaces;

internal interface IEmployeesRepository : IRepository
{
    Task<EmployeePasswordDto?> GetEmployeeDataByUserNameAsync(string userName);
    Task<Claim[]?> GetEmployeeClaimsByIdAsync(int id);
    Task SetEmployeePasswordAsync(int id, string hash);
}