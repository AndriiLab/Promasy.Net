namespace Promasy.Modules.Cpv.Dtos;

internal record CpvDto(int Id, string Code, string DescriptionEnglish, string DescriptionUkrainian, int Level,
    bool IsTerminal, int? ParentId);