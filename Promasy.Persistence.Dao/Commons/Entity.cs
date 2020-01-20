using System;
using Promasy.Persistence.Dao.Users;

namespace Promasy.Persistence.Dao.Commons
{
    public class Entity : BaseEntity
    {
        public bool Active { get; set; }
        
        public long Version { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        public long? CreatedBy { get; set; }
        
        public long? ModifiedBy { get; set; }
    }
}