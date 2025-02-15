using System;

namespace Promasy.Core.Exceptions;

public static class Ensure
{
    public static void FileNameSafety(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
        {
            throw new ArgumentException("Invalid file name");
        }
    }
}