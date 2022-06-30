using Microsoft.Extensions.Options;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Auth.Interfaces;

namespace Promasy.Modules.Auth.Services;

internal class TokenService : ITokenService
{
    private readonly IEmployeesRepository _employeesRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenSettings _settings;

    public TokenService(IOptions<TokenSettings> settings, IEmployeesRepository employeesRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        _employeesRepository = employeesRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _settings = settings.Value;
    }
    
    public async Task<UserTokens> GenerateTokenAsync(int employeeId)
    {
        var employee = await _employeesRepository.GetEmployeeByIdAsync(employeeId);
        var token = TokenHelper.GenerateJwtToken(employee!, _settings.Secret, _settings.JwtValidityMinutes);
        var refreshToken = TokenHelper.GenerateRefreshToken(employeeId, _settings.Secret, _settings.RefreshValidityMinutes);
        var expires = DateTime.UtcNow.AddMinutes(_settings.RefreshValidityMinutes);
        await _refreshTokenRepository.CreateAsync(refreshToken, employeeId, expires);

        return new UserTokens(token, refreshToken, expires);
    }

    public async Task<UserTokens?> RefreshTokenAsync(string refreshToken)
    {
        var employeeId = TokenHelper.ValidateAndGetUserId(refreshToken, _settings.Secret);
        if (employeeId is null)
        {
            return null;
        }
        
        var employee = await _employeesRepository.GetEmployeeByIdAsync(employeeId.Value);
        var newToken = TokenHelper.GenerateJwtToken(employee!, _settings.Secret, _settings.JwtValidityMinutes);
        var newRefreshToken = TokenHelper.GenerateRefreshToken(employeeId.Value, _settings.Secret, _settings.RefreshValidityMinutes);
        
        var expires = DateTime.UtcNow.AddMinutes(_settings.RefreshValidityMinutes);
        var isUpdated = await _refreshTokenRepository.UpdateAsync(refreshToken, newRefreshToken, expires);

        return isUpdated
            ? new UserTokens(newToken, newRefreshToken, DateTime.UtcNow.AddMinutes(_settings.RefreshValidityMinutes))
            : null;

    }

    public Task RevokeTokenAsync(string refreshToken)
    {
        return _refreshTokenRepository.RevokeAsync(refreshToken);
    }
}

public class TokenSettings
{
    public string Secret { get; set; }
    public int JwtValidityMinutes { get; set; }
    public int RefreshValidityMinutes { get; set; }
}