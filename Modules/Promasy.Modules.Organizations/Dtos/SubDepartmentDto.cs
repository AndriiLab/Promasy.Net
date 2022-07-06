using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Organizations.Dtos;

public record SubDepartmentDto(int Id, string Name, int DepartmentId, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);