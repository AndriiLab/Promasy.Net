using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Organizations.Dtos;

public record OrganizationDto(
    int Id,
    string Name,
    string Email,
    string Edrpou,
    string PhoneNumber,
    string? FaxNumber,
    string Country,
    string PostalCode,
    string Region,
    string City,
    int CityType,
    string Street,
    int StreetType,
    string BuildingNumber,
    string? InternalNumber,
    int EditorId = default,
    string Editor = "",
    DateTime EditedDate = default
    ) : OrganizationShortDto(Id, Name, EditorId, Editor, EditedDate);
    
public record OrganizationShortDto(
    int Id,
    string Name,
    int EditorId = default,
    string Editor = "",
    DateTime EditedDate = default
) : EntityDto(Id, EditorId, Editor, EditedDate);