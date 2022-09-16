using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Orders.Dtos;

namespace Promasy.Modules.Orders.Interfaces;

internal interface IReasonForSupplierChoiceRepository : IRepository
{
    Task<PagedResponse<ReasonForSupplierChoiceDto>> GetPagedListAsync(PagedRequest request);
    Task<ReasonForSupplierChoiceDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(ReasonForSupplierChoiceDto unit);
    Task UpdateAsync(ReasonForSupplierChoiceDto unit);
    Task MergeAsync(int targetId, int[] sourceIds);
}