using Promasy.Domain.Employees;
using Promasy.Modules.Orders.Dtos;

namespace Promasy.Modules.Orders.Interfaces;

internal interface IEmployeesRepository
{
    Task<List<EmployeeDto>> GetByIdsAndRolesAsync(IEnumerable<Tuple<int, RoleName>> idRoles);
    Task<List<EmployeeDto>> GetEmployeesForDepartmentIdAsync(int departmentId, params RoleName[] roles);
}