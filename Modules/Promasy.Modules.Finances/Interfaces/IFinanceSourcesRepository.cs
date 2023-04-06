using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Finances.Dtos;
using Promasy.Modules.Finances.Models;

namespace Promasy.Modules.Finances.Interfaces;

public interface IFinanceSourcesRepository : IRepository
{
    Task<PagedResponse<FinanceSourceShortDto>> GetPagedListAsync(FinanceSourcesPagedRequest request);
    Task<FinanceSourceDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateFinanceSourceDto item);
    Task UpdateAsync(UpdateFinanceSourceDto item);
    Task DeleteByIdAsync(int id);
}