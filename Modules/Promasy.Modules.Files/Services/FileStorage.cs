using Promasy.Application.Interfaces;
using Promasy.Core.Exceptions;

namespace Promasy.Modules.Files.Services;

internal class FileStorage : IFileStorage
{
    private const string ReportsPath = "Reports";

    public Task<byte[]> ReadFileAsync(string fileName)
    {
        Ensure.FileNameSafety(fileName);

        var path = Path.Combine(Directory.GetCurrentDirectory(), ReportsPath, fileName);
        return File.Exists(path)
            ? File.ReadAllBytesAsync(path)
            : Task.FromResult(Array.Empty<byte>());
    }

    public string GetPathForFile(string fileName)
    {
        Ensure.FileNameSafety(fileName);

        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), ReportsPath)))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), ReportsPath));
        }

        return Path.Combine(Directory.GetCurrentDirectory(), ReportsPath, fileName);
    }
}