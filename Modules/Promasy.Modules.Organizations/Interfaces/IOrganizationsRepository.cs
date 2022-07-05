using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface IOrganizationsRepository : IRepository
{
    Task<PagedResponse<OrganizationShortDto>> GetPagedListAsync(PagedRequest request);
    Task<OrganizationDto?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
    Task<int> CreateAsync(OrganizationDto item);
    Task UpdateAsync(OrganizationDto item);
}