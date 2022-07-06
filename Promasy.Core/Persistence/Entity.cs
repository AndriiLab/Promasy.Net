using System;

namespace Promasy.Core.Persistence;

public class Entity : IBaseEntity, ISoftDeletable
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool Deleted { get; set; }
    public int CreatorId { get; set; }
    public int? ModifierId { get; set; }
}