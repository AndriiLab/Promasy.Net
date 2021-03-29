using Microsoft.AspNetCore.Identity;

namespace Promasy.Domain.Users
{
    public class Role : IdentityRole<int>
    {
        private Role()
        {
        }

        public Role(string name) : base(name)
        {
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
}