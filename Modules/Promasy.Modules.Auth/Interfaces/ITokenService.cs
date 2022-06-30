using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Auth.Interfaces;

internal interface ITokenService : IService
{
    Task<UserTokens> GenerateTokenAsync(int employeeId);
    Task<UserTokens?> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
}

public record UserTokens(string Token, string RefreshToken, DateTime RefreshExpiryTime);