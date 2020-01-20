using System.Collections.Generic;
using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;

namespace Promasy.Persistence.Dao.Institutes
{
    public class Institute : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Edrpou { get; set; }
        public string FaxNumber { get; set; }
        public string PhoneNumber { get; set; }

        public long AddressId { get; set; }
        public virtual Address Address { get; set; }
        
        public virtual ICollection<Department> Departments { get; set; }
    }
}
