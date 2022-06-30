using System.Collections.Generic;

namespace Promasy.Domain.Employees;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Employee> Employees = new List<Employee>();

    private Role()
    {
    }

    public Role(string name)
    {
        Name = name;
    }
}

public static class RoleName
{
    public const string Administrator = "Адміністратор";
    public const string Director = "Директор";
    public const string DeputyDirector = "Заступник директора";
    public const string ChiefEconomist = "Головний економіст";
    public const string ChiefAccountant = "Головний бухгалтер";
    public const string HeadOfTenderCommittee = "Голова тендерного комітету";
    public const string SecretaryOfTenderCommittee = "Секретар тендерного комітету";
    public const string HeadOfDepartment = "Керівник підрозділу";
    public const string PersonallyLiableEmployee = "Матеріально-відповідальна особа";
    public const string User = "Користувач";
}