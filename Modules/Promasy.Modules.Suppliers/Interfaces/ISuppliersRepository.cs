using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Suppliers.Dtos;

namespace Promasy.Modules.Suppliers.Interfaces;

internal interface ISuppliersRepository : IRepository
{
    Task<PagedResponse<SupplierDto>> GetPagedListAsync(PagedRequest request);
    Task<SupplierDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(SupplierDto supplier);
    Task UpdateAsync(SupplierDto unit);
    Task MergeAsync(int targetId, int[] sourceIds);
}