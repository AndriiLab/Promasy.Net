using Promasy.Core.Persistence;

namespace Promasy.Domain.Suppliers;

public class Supplier : OrganizationEntity
{
    public string Name { get; set; }
    public string? Comment { get; set; }
    public string? Phone { get; set; }
}