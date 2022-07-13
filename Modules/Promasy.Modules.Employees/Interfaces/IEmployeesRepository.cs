using Promasy.Modules.Core.Responses;
using Promasy.Modules.Employees.Dtos;
using Promasy.Modules.Employees.Models;

namespace Promasy.Modules.Employees.Interfaces;

internal interface IEmployeesRepository
{
    Task<PagedResponse<EmployeeShortDto>> GetPagedListAsync(EmployeesPagedRequest request);
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateEmployeeDto employee);
    Task UpdateAsync(UpdateEmployeeDto employee);
    Task DeleteByIdAsync(int id);
}