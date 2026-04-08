using Microsoft.AspNetCore.Hosting;
using Promasy.Application.Interfaces;
using Promasy.Core.Exceptions;

namespace Promasy.Modules.Files.Services;

internal class FileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _environment;
    private const string ReportsPath = "Reports";

    public FileStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<byte[]> ReadFileAsync(string fileName)
    {
        Ensure.FileNameSafety(fileName);

        var path = Path.Combine(_environment.ContentRootPath, ReportsPath, fileName);
        return File.Exists(path)
            ? File.ReadAllBytesAsync(path)
            : Task.FromResult(Array.Empty<byte>());
    }

    public string GetPathForFile(string fileName)
    {
        Ensure.FileNameSafety(fileName);

        if (!Directory.Exists(Path.Combine(_environment.ContentRootPath, ReportsPath)))
        {
            Directory.CreateDirectory(Path.Combine(_environment.ContentRootPath, ReportsPath));
        }

        return Path.Combine(_environment.ContentRootPath, ReportsPath, fileName);
    }
}