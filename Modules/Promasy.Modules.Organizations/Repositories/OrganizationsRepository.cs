using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Application.Persistence;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Pagination;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Repositories;

internal class OrganizationsRepository : IOrganizationRules, IOrganizationsRepository
{
    private readonly IDatabase _database;

    public OrganizationsRepository(IDatabase database)
    {
        _database = database;
    }

    public Task<bool> IsExistsAsync(int id, CancellationToken ct)
    {
        return _database.Organizations.AnyAsync(o => o.Id == id, ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct)
    {
        return await _database.Organizations.AnyAsync(o => EF.Functions.ILike(o.Name, name), ct) == false;
    }

    public async Task<bool> IsNameUniqueAsync(string name, int id, CancellationToken ct)
    {
        return await _database.Organizations.Where(o => o.Id != id)
            .AnyAsync(o => EF.Functions.ILike(o.Name, name), ct) == false;
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
            query = query.Where(u => EF.Functions.ILike(u.Name, $"%{request.Search}%"));
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
        entity.ModifiedDate = DateTime.UtcNow;
        entity.Address.Country = organization.Country;
        entity.Address.PostalCode = organization.PostalCode;
        entity.Address.Region = organization.Region;
        entity.Address.City = organization.City;
        entity.Address.CityType = (CityType) organization.CityType;
        entity.Address.Street = organization.Street;
        entity.Address.StreetType = (StreetType) organization.StreetType;
        entity.Address.BuildingNumber = organization.BuildingNumber;
        entity.Address.InternalNumber = organization.InternalNumber;
        entity.Address.ModifiedDate = DateTime.UtcNow;

        await _database.SaveChangesAsync();
    }

    public Task<bool> IsSameOrganizationAsync(int id, int userOrganizationId, CancellationToken ct)
    {
        return Task.FromResult(id == userOrganizationId);
    }

    public Task<bool> IsSameDepartmentAsync(int id, int userDepartmentId, CancellationToken ct)
    {
        throw new NotSupportedException();
    }

    public Task<bool> IsSameSubDepartmentAsync(int id, int userSubDepartmentId, CancellationToken ct)
    {
        throw new NotSupportedException();
    }

    public Task<bool> IsSameUserAsync(int id, int userId, CancellationToken ct)
    {
        throw new NotSupportedException();
    }
}