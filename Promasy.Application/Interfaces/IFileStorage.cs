namespace Promasy.Application.Interfaces;

public interface IFileStorage
{
    Task<byte[]> ReadFileAsync(string fileName);
    string GetPathForFile(string fileName);
}