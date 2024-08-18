namespace Promasy.Modules.Core.Permissions;

public class PermissionTag
{
    public string Name { get; }
    private PermissionTag(string name)
    {
        Name = name;
    }
    
    public static PermissionTag List(string endpoint) => new($"List/{endpoint}");
    public static PermissionTag Get(string endpoint) => new($"Get/{endpoint}");
    public static PermissionTag Create(string endpoint) => new($"Create/{endpoint}");
    public static PermissionTag Update(string endpoint) => new($"Update/{endpoint}");
    public static PermissionTag Delete(string endpoint) => new($"Delete/{endpoint}");
    public static PermissionTag Merge(string endpoint) => new($"Merge/{endpoint}");
    public static PermissionTag Export(string endpoint) => new($"Export/{endpoint}");
}