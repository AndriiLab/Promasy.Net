using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Finances.Dtos;

namespace Promasy.Modules.Finances.Interfaces;

public interface IFinanceFinanceSubDepartmentsRepository : IRepository
{
    Task<PagedResponse<FinanceSubDepartmentDto>> GetPagedListAsync(int financeSourceId, PagedRequest request);
    Task<PagedResponse<FinanceSubDepartmentDto>> GetPagedListBySubDepartmentAsync(int subDepartmentId, PagedRequest request);
    Task<FinanceSubDepartmentDto?> GetByFinanceSubDepartmentIdsAsync(int financeId, int subDepartmentId);
    Task<int> CreateAsync(CreateFinanceSubDepartmentDto item);
    Task UpdateAsync(UpdateFinanceSubDepartmentDto item);
    Task DeleteByFinanceSubDepartmentIdsAsync(int financeId, int subDepartmentId);
}