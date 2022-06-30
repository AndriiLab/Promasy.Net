using Promasy.Modules.Auth.Dtos;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Auth.Interfaces;

internal interface IEmployeesRepository : IRepository
{
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
}