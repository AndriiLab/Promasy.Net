namespace Promasy.Modules.Units.Dtos;

internal record UnitDto(int Id, string Name, int? EditorId = null, string? Editor = null, DateTime? EditedDate = null);
