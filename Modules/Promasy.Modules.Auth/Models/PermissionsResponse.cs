using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Auth.Models;

public record PermissionsResponse(IReadOnlyCollection<EndpointPermission> Permissions);