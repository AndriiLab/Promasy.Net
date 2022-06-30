using System.Reflection;
using System.Threading.Tasks;

namespace Promasy.Core.Extensions;

public static class ReflectionExtensions
{
    public static async Task<object> InvokeAsync(this MethodInfo mi, object obj, params object[] parameters)
    {
        dynamic awaitable = mi.Invoke(obj, parameters);
        await awaitable;
        return awaitable.GetAwaiter().GetResult();
    }
}