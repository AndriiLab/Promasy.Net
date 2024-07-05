namespace Promasy.Domain.Employees;

public enum RoleName
{
    Administrator = 1,
    Director = 2,
    DeputyDirector = 3,
    HeadOfTenderCommittee = 4,
    SecretaryOfTenderCommittee = 5,
    ChiefAccountant = 6,
    ChiefEconomist = 7,
    HeadOfDepartment = 8,
    PersonallyLiableEmployee = 9,
    User = 10
}

public static class RoleNames
{
    public static readonly RoleName[] AllExceptUser =
    [
        RoleName.Administrator, RoleName.Director, RoleName.DeputyDirector, RoleName.HeadOfTenderCommittee,
        RoleName.SecretaryOfTenderCommittee, RoleName.ChiefAccountant, RoleName.ChiefEconomist,
        RoleName.HeadOfDepartment,
        RoleName.PersonallyLiableEmployee
    ];
}