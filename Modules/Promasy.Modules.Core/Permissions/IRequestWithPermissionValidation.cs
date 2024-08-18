namespace Promasy.Modules.Core.Permissions;

public interface IRequestWithPermissionValidation
{
    int GetId();
}

public interface IRequestWithMultiplePermissionValidation : IRequestWithPermissionValidation
{
    int[] GetIds();
}