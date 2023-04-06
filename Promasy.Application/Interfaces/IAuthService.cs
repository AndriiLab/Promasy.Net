namespace Promasy.Application.Interfaces;

public interface IAuthService : IService
{
    Task<int?> AuthAsync(string userName, string password);
    Task SetUserContextAsync(int id);
    Task SetEmployeePasswordAsync(int id, string password);
}