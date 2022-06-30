namespace Promasy.Domain.Vocabulary;

public class Cpv
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string DescriptionEnglish { get; set; }
    public string DescriptionUkrainian { get; set; }
    public int Level { get; set; }
    public bool IsTerminal { get; set; }

    public int? ParentId { get; set; }
    public Cpv Parent { get; set; }
}