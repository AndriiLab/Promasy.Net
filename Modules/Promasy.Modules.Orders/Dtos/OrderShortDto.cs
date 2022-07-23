using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Orders.Dtos;

public record OrderShortDto(int Id, string Description, decimal Total, int Status,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);