using System;
using Microsoft.AspNetCore.Identity;
using Promasy.Common.Persistence;
using Promasy.Domain.Institutes;

namespace Promasy.Domain.Users
{
    public class Employee : IdentityUser<int>, IEntity
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneReserve { get; set; }

        public int SubDepartmentId { get; set; }
        public virtual SubDepartment SubDepartment { get; set; }
        public Employee Creator { get; set; }
        public Employee Modifier { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Deleted { get; set; }
        public int? CreatorId { get; set; }

        public int? ModifierId { get; set; }
    }
}