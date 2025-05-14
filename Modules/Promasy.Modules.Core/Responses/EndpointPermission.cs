using Promasy.Modules.Core.Permissions;

namespace Promasy.Modules.Core.Responses;

public record EndpointPermission(string Tag, PermissionAction Action, PermissionCondition Condition);