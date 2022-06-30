using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Units.Dtos;

namespace Promasy.Modules.Units.Interfaces;

internal interface IUnitsRepository : IRepository
{
    Task<PagedResponse<UnitDto>> GetUnitsAsync(PagedRequest request);
    Task<UnitDto?> GetUnitByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateUnitAsync(UnitDto unit);
    Task UpdateUnitAsync(UnitDto unit);
}