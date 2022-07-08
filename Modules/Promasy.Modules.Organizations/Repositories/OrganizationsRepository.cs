using Microsoft.EntityFrameworkCore;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Domain.Persistence;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Repositories;

public class OrganizationsRepository : IOrganizationsRules, IOrganizationsRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;

    public OrganizationsRepository(IDatabase database, IUserContext userContext)
    {
        _database = database;
        _userContext = userContext;
    }

    public Task<bool> IsExistAsync(int id, CancellationToken ct)
    {
        return _database.Organizations.AnyAsync(o => o.Id == id, ct);
    }

    public Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return _database.Organizations.AllAsync(o => o.Name != name, ct);
    }

    public Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return _database.Organizations.Where(o => o.Id != id).AllAsync(o => o.Name != name, ct);
    }

    public bool IsEditable(int id)
    {
        return _userContext.HasRoles((int) RoleName.Administrator) ||
               (_userContext.HasRoles((int)RoleName.Director, (int)RoleName.DeputyDirector) && _userContext.GetOrganizationId() == id);
    }

    public Task<bool> IsUsedAsync(int id, CancellationToken ct)
    {
        return _database.Departments.AnyAsync(o => o.OrganizationId == id, ct);
    }

    public Task<PagedResponse<OrganizationShortDto>> GetPagedListAsync(PagedRequest request)
    {
        var query = _database.Organizations
            .AsNoTracking();
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => u.Name.Contains(request.Search));
        }

        return query
            .PaginateAsync(request,
                o => new OrganizationShortDto(o.Id, o.Name, o.ModifierId ?? o.CreatorId,
                    PromasyDbFunction.GetEmployeeShortName(o.ModifierId ?? o.CreatorId),
                    o.ModifiedDate ?? o.CreatedDate));
    }

    public Task<OrganizationDto?> GetByIdAsync(int id)
    {
        return _database.Organizations
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrganizationDto(o.Id, o.Name, o.Email, o.Edrpou, o.PhoneNumber, o.FaxNumber,
                o.Address.Country, o.Address.PostalCode, o.Address.Region, o.Address.City, (int)o.Address.CityType,
                o.Address.Street, (int)o.Address.StreetType, o.Address.BuildingNumber, o.Address.InternalNumber,
                o.ModifierId ?? o.CreatorId, PromasyDbFunction.GetEmployeeShortName(o.ModifierId ?? o.CreatorId),
                o.ModifiedDate ?? o.CreatedDate))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var organization = await _database.Organizations.FirstOrDefaultAsync(o => o.Id == id);
        if (organization is null)
        {
            return;
        }

        _database.Organizations.Remove(organization);
        await _database.SaveChangesAsync();
    }

    public async Task<int> CreateAsync(OrganizationDto organization)
    {
        var entity = new Organization
        {
            Name = organization.Name,
            Email = organization.Email,
            Edrpou = organization.Edrpou,
            PhoneNumber = organization.PhoneNumber,
            FaxNumber = organization.FaxNumber,
            Address = new Address
            {
                Country = organization.Country,
                PostalCode = organization.PostalCode,
                Region = organization.Region,
                City = organization.City,
                CityType = (CityType)organization.CityType,
                Street = organization.Street,
                StreetType = (StreetType)organization.StreetType,
                BuildingNumber = organization.BuildingNumber,
                InternalNumber = organization.InternalNumber,
            }
        };
        _database.Organizations.Add(entity);
        await _database.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateAsync(OrganizationDto organization)
    {
        var entity = await _database.Organizations
            .Include(o => o.Address)
            .FirstAsync(o => o.Id == organization.Id);
        entity.Name = organization.Name;
        entity.Email = organization.Email;
        entity.Edrpou = organization.Edrpou;
        entity.PhoneNumber = organization.PhoneNumber;
        entity.FaxNumber = organization.FaxNumber;
        entity.Address.Country = organization.Country;
        entity.Address.PostalCode = organization.PostalCode;
        entity.Address.Region = organization.Region;
        entity.Address.City = organization.City;
        entity.Address.CityType = (CityType) organization.CityType;
        entity.Address.Street = organization.Street;
        entity.Address.StreetType = (StreetType) organization.StreetType;
        entity.Address.BuildingNumber = organization.BuildingNumber;
        entity.Address.InternalNumber = organization.InternalNumber;

        await _database.SaveChangesAsync();
    }
}