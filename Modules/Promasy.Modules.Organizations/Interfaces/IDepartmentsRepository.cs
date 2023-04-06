using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface IDepartmentsRepository : IRepository
{
    Task<PagedResponse<DepartmentDto>> GetPagedListAsync(PagedRequest request);
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(DepartmentDto item);
    Task UpdateAsync(DepartmentDto item);
}