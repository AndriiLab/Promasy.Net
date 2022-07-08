using System.Security.Claims;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Auth.Interfaces;

internal interface IEmployeesRepository : IRepository
{
    Task<Claim[]?> GetEmployeeClaimsByIdAsync(int id);
}