using System.Collections.Generic;

namespace Promasy.Application.Interfaces
{
    public interface IUserContext
    {
        int Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        ICollection<string> Roles { get; }
    }
}