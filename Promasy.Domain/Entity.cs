using System;
using Promasy.Common.Persistence;
using Promasy.Domain.Users;

namespace Promasy.Domain
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Deleted { get; set; }
        
        public int? CreatorId { get; set; }
        public Employee Creator { get; set; }
        
        public int? ModifierId { get; set; }
        public Employee Modifier { get; set; }
    }
}