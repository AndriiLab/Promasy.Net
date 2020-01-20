using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;

namespace Promasy.Persistence.Dao.Suppliers
{
    public class Supplier : Entity
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Phone { get; set; }
    }
}
