using Promasy.Common.Persistence;

namespace Promasy.Domain.Suppliers
{
    public class Supplier : Entity
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Phone { get; set; }
    }
}
