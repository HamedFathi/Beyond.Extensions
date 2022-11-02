// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.FileInfoExtended;

public static class FileInfoExtensions
{
    public static void AppendAllLines(this FileInfo @this, IEnumerable<string> contents)
    {
        File.AppendAllLines(@this.FullName, contents);
    }

    public static void AppendAllLines(this FileInfo @this, IEnumerable<string> contents, Encoding encoding)
    {
        File.AppendAllLines(@this.FullName, contents, encoding);
    }

    public static void AppendAllText(this FileInfo @this, string contents)
    {
        File.AppendAllText(@this.FullName, contents);
    }

    public static void AppendAllText(this FileInfo @this, string contents, Encoding encoding)
    {
        File.AppendAllText(@this.FullName, contents, encoding);
    }

    public static string ChangeExtension(this FileInfo @this, string extension)
    {
        return Path.ChangeExtension(@this.FullName, extension);
    }

    public static int CountLines(this FileInfo @this)
    {
        return File.ReadAllLines(@this.FullName).Length;
    }

    public static int CountLines(this FileInfo @this, Func<string, bool> predicate)
    {
        return File.ReadAllLines(@this.FullName).Count(predicate);
    }

    public static DirectoryInfo CreateDirectory(this FileInfo @this)
    {
        return Directory.CreateDirectory(@this.Directory?.FullName ?? throw new InvalidOperationException());
    }

    public static DirectoryInfo EnsureDirectoryExists(this FileInfo @this)
    {
        return Directory.CreateDirectory(@this.Directory?.FullName ?? throw new InvalidOperationException());
    }

    public static string? GetDirectoryFullName(this FileInfo @this)
    {
        return @this.Directory?.FullName;
    }

    public static string? GetDirectoryName(this FileInfo @this)
    {
        return @this.Directory?.Name;
    }

    public static string GetFileNameWithoutExtension(this FileInfo @this)
    {
        return Path.GetFileNameWithoutExtension(@this.FullName);
    }

    public static string? GetPathRoot(this FileInfo @this)
    {
        return Path.GetPathRoot(@this.FullName);
    }

    public static bool HasExtension(this FileInfo @this)
    {
        return Path.HasExtension(@this.FullName);
    }

    public static bool IsPathRooted(this FileInfo @this)
    {
        return Path.IsPathRooted(@this.FullName);
    }

    public static void MoveTo(this FileInfo fileInfo, string destFileName, bool renameWhenExists = false)
    {
        var newFullPath = string.Empty;

        if (renameWhenExists)
        {
            var count = 1;

            var fileNameOnly = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            var extension = Path.GetExtension(fileInfo.FullName);
            newFullPath = Path.Combine(destFileName, fileInfo.Name);

            while (File.Exists(newFullPath))
            {
                var tempFileName = $"{fileNameOnly}({count++})";
                newFullPath = Path.Combine(destFileName, tempFileName + extension);
            }
        }

        fileInfo.MoveTo(renameWhenExists ? newFullPath : destFileName);
    }

    public static byte[] ReadAllBytes(this FileInfo @this)
    {
        return File.ReadAllBytes(@this.FullName);
    }

    public static string[] ReadAllLines(this FileInfo @this)
    {
        return File.ReadAllLines(@this.FullName);
    }

    public static string[] ReadAllLines(this FileInfo @this, Encoding encoding)
    {
        return File.ReadAllLines(@this.FullName, encoding);
    }

    public static string ReadAllText(this FileInfo @this)
    {
        return File.ReadAllText(@this.FullName);
    }

    public static string ReadAllText(this FileInfo @this, Encoding encoding)
    {
        return File.ReadAllText(@this.FullName, encoding);
    }

    public static IEnumerable<string> ReadLines(this FileInfo @this)
    {
        return File.ReadLines(@this.FullName);
    }

    public static IEnumerable<string> ReadLines(this FileInfo @this, Encoding encoding)
    {
        return File.ReadLines(@this.FullName, encoding);
    }

    public static string ReadToEnd(this FileInfo @this)
    {
        using var stream = File.Open(@this.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public static string ReadToEnd(this FileInfo @this, long position)
    {
        using var stream = File.Open(@this.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.Position = position;

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public static string ReadToEnd(this FileInfo @this, Encoding encoding)
    {
        using var stream = File.Open(@this.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream, encoding);
        return reader.ReadToEnd();
    }

    public static string ReadToEnd(this FileInfo @this, Encoding encoding, long position)
    {
        using var stream = File.Open(@this.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.Position = position;

        using var reader = new StreamReader(stream, encoding);
        return reader.ReadToEnd();
    }

    public static void Rename(this FileInfo @this, string newName)
    {
        var filePath = Path.Combine(@this.Directory?.FullName ?? throw new InvalidOperationException(), newName);
        @this.MoveTo(filePath);
    }

    public static void RenameExtension(this FileInfo @this, string extension)
    {
        var filePath = Path.ChangeExtension(@this.FullName, extension);
        @this.MoveTo(filePath);
    }

    public static void RenameFileWithoutExtension(this FileInfo @this, string newName)
    {
        var fileName = string.Concat(newName, @this.Extension);
        var filePath = Path.Combine(@this.Directory?.FullName ?? throw new InvalidOperationException(), fileName);
        @this.MoveTo(filePath);
    }

    public static FileInfo[] SetAttributes(this FileInfo[] files, FileAttributes attributes)
    {
        foreach (var file in files)
            file.Attributes = attributes;
        return files;
    }

    public static FileInfo[] SetAttributesAdditive(this FileInfo[] files, FileAttributes attributes)
    {
        foreach (var file in files)
            file.Attributes = file.Attributes | attributes;
        return files;
    }

    public static void WriteAllBytes(this FileInfo @this, byte[] bytes)
    {
        File.WriteAllBytes(@this.FullName, bytes);
    }

    public static void WriteAllLines(this FileInfo @this, string[] contents)
    {
        File.WriteAllLines(@this.FullName, contents);
    }

    public static void WriteAllLines(this FileInfo @this, string[] contents, Encoding encoding)
    {
        File.WriteAllLines(@this.FullName, contents, encoding);
    }

    public static void WriteAllLines(this FileInfo @this, IEnumerable<string> contents)
    {
        File.WriteAllLines(@this.FullName, contents);
    }

    public static void WriteAllLines(this FileInfo @this, IEnumerable<string> contents, Encoding encoding)
    {
        File.WriteAllLines(@this.FullName, contents, encoding);
    }

    public static void WriteAllText(this FileInfo @this, string contents)
    {
        File.WriteAllText(@this.FullName, contents);
    }

    public static void WriteAllText(this FileInfo @this, string contents, Encoding encoding)
    {
        File.WriteAllText(@this.FullName, contents, encoding);
    }
}