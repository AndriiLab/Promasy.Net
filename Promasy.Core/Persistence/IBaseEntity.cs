using System;

namespace Promasy.Core.Persistence;

public interface IBaseEntity
{
    int Id { get; set; }
    DateTime CreatedDate { get; set; }
    DateTime? ModifiedDate { get; set; }
    int CreatorId { get; set; }
    int? ModifierId { get; set; }
}