using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface ISubDepartmentsRepository : IRepository
{
    Task<PagedResponse<SubDepartmentDto>> GetPagedListAsync(int departmentId, PagedRequest request);
    Task<SubDepartmentDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(SubDepartmentDto item);
    Task UpdateAsync(SubDepartmentDto item);
}