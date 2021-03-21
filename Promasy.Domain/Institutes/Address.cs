using Promasy.Common.Persistence;

namespace Promasy.Domain.Institutes
{
    public class Address : Entity
    {
        public string BuildingNumber { get; set; }
        public string City { get; set; }
        public string CityType { get; set; }
        public string CorpusNumber { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        public string StreetType { get; set; }
    }
}
