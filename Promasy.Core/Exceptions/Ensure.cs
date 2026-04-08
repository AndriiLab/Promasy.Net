using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Promasy.Core.Exceptions;

public static class Ensure
{
    private static readonly HashSet<string> ReservedNames =
    [
        "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1",
        "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    ];

    public static void FileNameSafety(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty or whitespace", nameof(fileName));
        }

        if (fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            throw new ArgumentException("File name contains invalid characters", nameof(fileName));
        }

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName).ToUpperInvariant();
        if (ReservedNames.Contains(nameWithoutExtension))
        {
            throw new ArgumentException($"File name '{nameWithoutExtension}' is a reserved system name", nameof(fileName));
        }

        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
        {
            throw new ArgumentException("Invalid file name: path traversals or separators are not allowed", nameof(fileName));
        }
    }
}