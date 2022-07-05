using Promasy.Core.Persistence;

namespace Promasy.Domain.Organizations;

public class Address : Entity
{
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public CityType CityType { get; set; }
    public string Street { get; set; }
    public StreetType StreetType { get; set; }
    public string BuildingNumber { get; set; }
    public string? InternalNumber { get; set; }
}