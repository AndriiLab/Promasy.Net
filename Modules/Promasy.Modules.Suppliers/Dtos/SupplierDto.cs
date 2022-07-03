using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Suppliers.Dtos;

internal record SupplierDto(int Id, string Name, string? Comment, string? Phone, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);
