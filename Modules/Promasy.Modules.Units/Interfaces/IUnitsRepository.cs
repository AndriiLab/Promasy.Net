using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Units.Dtos;

namespace Promasy.Modules.Units.Interfaces;

internal interface IUnitsRepository : IRepository
{
    Task<PagedResponse<UnitDto>> GetPagedListAsync(PagedRequest request);
    Task<UnitDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(UnitDto unit);
    Task UpdateAsync(UnitDto unit);
    Task MergeAsync(int targetId, int[] sourceIds);
}