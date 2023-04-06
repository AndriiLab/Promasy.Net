using System.Net.Mime;
using Promasy.Core.Exceptions;

namespace Promasy.Modules.Files.Helpers;

public static class MediaTypeNameHelper
{
    public static string GetMediaTypeName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        if (string.IsNullOrEmpty(extension))
        {
            throw new ServiceException("File extension not defined");
        }

        return extension switch
        {
            ".pdf" => MediaTypeNames.Application.Pdf,
            ".csv" => "text/csv",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => throw new ServiceException($"Unknown MIME for file extension {extension}")
        };
    }
}