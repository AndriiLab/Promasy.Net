using Promasy.Persistence.Dao.Common;
using Promasy.Persistence.Dao.Commons;
using Promasy.Persistence.Dao.Institutes;

namespace Promasy.Persistence.Dao.Users
{
    public class Employee : Entity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PhoneMain { get; set; }
        public string PhoneReserve { get; set; }
        public string Role { get; set; }
        public long Salt { get; set; }
        
        public long SubdepartmentId { get; set; }
        public virtual Subdepartment Subdepartment { get; set; }
    }
}
