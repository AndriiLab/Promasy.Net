
namespace Promasy.Core.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string name)
    {
        return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name).Replace(" ", string.Empty);
    }
}