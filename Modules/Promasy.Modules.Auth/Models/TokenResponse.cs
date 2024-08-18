using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Auth.Models;

public record TokenResponse(string Token);

public record AuthResponse(string Token, IReadOnlyCollection<EndpointPermission> Permissions) : TokenResponse(Token);