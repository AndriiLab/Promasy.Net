using System;

namespace Promasy.Common.Persistence
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        bool Deleted { get; set; }
        int? CreatorId { get; set; }
        int? ModifierId { get; set; }
    }
}