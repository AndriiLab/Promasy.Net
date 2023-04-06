using Promasy.Application.Interfaces;

namespace Promasy.Modules.Files.Services;

internal class FileStorage : IFileStorage
{
    private const string ReportsPath = "Reports";

    public Task<byte[]> ReadFileAsync(string fileName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), ReportsPath, fileName);
        if (!File.Exists(path))
        {
            return Task.FromResult(Array.Empty<byte>());
        }

        return File.ReadAllBytesAsync(path);
    }

    public string GetPathForFile(string fileName)
    {
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), ReportsPath)))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), ReportsPath));
        }

        return Path.Combine(Directory.GetCurrentDirectory(), ReportsPath, fileName);
    }
}