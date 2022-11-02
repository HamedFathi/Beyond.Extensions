// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

using Beyond.Extensions.ByteArrayExtended;
using Beyond.Extensions.Enums;
using Beyond.Extensions.ObjectExtended;
using Beyond.Extensions.StringExtended;

#pragma warning disable SYSLIB0001

namespace Beyond.Extensions.StreamExtended;

public static class StreamExtensions
{
    public static void Clear(this Stream stream)
    {
        stream.SetLength(0);
    }

    public static T? ConvertTo<T>(this Stream @this)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        var byteArray = @this.ToByteArrayByJsonSerializer();
        return byteArray.ConvertTo<T>();
    }

    public static void CopyTo(this Stream fromStream, Stream toStream)
    {
        if (fromStream == null)
            throw new ArgumentNullException(nameof(fromStream));
        if (toStream == null)
            throw new ArgumentNullException(nameof(toStream));
        var bytes = new byte[8092];
        int dataRead;
        while ((dataRead = fromStream.Read(bytes, 0, bytes.Length)) > 0)
            toStream.Write(bytes, 0, dataRead);
    }

    public static Stream CopyTo(this Stream stream, Stream targetStream, int bufferSize)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (targetStream == null) throw new ArgumentNullException(nameof(targetStream));
        if (!stream.CanRead)
            throw new InvalidOperationException("Source stream does not support reading.");
        if (!targetStream.CanWrite)
            throw new InvalidOperationException("Target stream does not support writing.");
        var buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0)
            targetStream.Write(buffer, 0, bytesRead);
        return stream;
    }

    public static MemoryStream CopyToMemory(this Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var memoryStream = new MemoryStream((int)stream.Length);
        stream.CopyTo(memoryStream);
        return memoryStream;
    }

    public static Encoding GetEncoding(this FileStream file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        // Read the BOM
        var bom = new byte[4];
        // ReSharper disable once MustUseReturnValue
        file.Read(bom, 0, 4);
        // Analyze the BOM
        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
        return Encoding.ASCII;
    }

    public static StreamReader GetReader(this Stream stream)
    {
        return stream.GetReader(Encoding.UTF8);
    }

    public static StreamReader GetReader(this Stream stream, Encoding encoding)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (encoding == null) throw new ArgumentNullException(nameof(encoding));
        if (!stream.CanRead)
            throw new InvalidOperationException("Stream does not support reading.");
        return new StreamReader(stream, encoding);
    }

    public static StreamWriter GetWriter(this Stream stream)
    {
        return stream.GetWriter(Encoding.UTF8);
    }

    public static StreamWriter GetWriter(this Stream stream, Encoding encoding)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (encoding == null) throw new ArgumentNullException(nameof(encoding));
        if (!stream.CanWrite)
            throw new InvalidOperationException("Stream does not support writing.");
        return new StreamWriter(stream, encoding);
    }

    public static bool IsEmpty(this Stream stream)
    {
        return stream.Length <= 0;
    }

    public static bool IsNotEmpty(this Stream stream)
    {
        return !stream.IsEmpty();
    }

    public static bool IsNotNull(this Stream? stream)
    {
        return stream != null;
    }

    public static bool IsNotNullOrEmpty(this Stream? stream)
    {
        return !stream.IsNullOrEmpty();
    }

    public static bool IsNull(this Stream? stream)
    {
        return stream == null;
    }

    public static bool IsNullOrEmpty(this Stream? stream)
    {
        if (stream == null)
            return true;
        return stream.Length <= 0;
    }

    public static string ReadAll(this Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var sr = new StreamReader(stream);
        return sr.ReadToEnd();
    }

    public static byte[] ReadAllBytes(this Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using var memoryStream = stream.CopyToMemory();
        return memoryStream.ToArray();
    }

    public static void ReadBlock(this Stream stream, Action<string> action, int bufferSize = 1024,
        EncodingType encoding = EncodingType.UTF8)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (action == null) throw new ArgumentNullException(nameof(action));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = stream.Read(buffer, 0, bufferSize);
            if (count == 0) break;
            var text = buffer.ToText(0, count, encoding);
            action(text);
        }
    }

    public static IEnumerable<string> ReadBlock(this Stream stream, int bufferSize = 1024,
        EncodingType encoding = EncodingType.UTF8)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = stream.Read(buffer, 0, bufferSize);
            if (count == 0) break;
            var text = buffer.ToText(0, count, encoding);
            yield return text;
        }
    }

    public static void ReadBlock(this Stream stream, Action<byte[]> action, int bufferSize = 1024)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (action == null) throw new ArgumentNullException(nameof(action));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = stream.Read(buffer, 0, bufferSize);
            if (count == 0) break;
            action(buffer);
        }
    }

    public static IEnumerable<byte[]> ReadBlockAsBytes(this Stream stream, int bufferSize = 1024)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = stream.Read(buffer, 0, bufferSize);
            if (count == 0) break;
            yield return buffer;
        }
    }

    public static async IAsyncEnumerable<byte[]> ReadBlockAsBytesAsync(this Stream stream, int bufferSize = 1024)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = await stream.ReadAsync(buffer, 0, bufferSize);
            if (count == 0) break;
            yield return buffer;
        }
    }

    public static string ReadBlockAsString(this Stream stream, int bufferSize = 1024,
        EncodingType encoding = EncodingType.UTF8)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var sb = new StringBuilder();
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = stream.Read(buffer, 0, bufferSize);
            if (count == 0) break;
            sb.Append(buffer.ToText(encoding).ToCharArray(), 0, count);
        }

        var text = sb.ToString();
        return text;
    }

    public static async Task<string> ReadBlockAsStringAsync(this Stream stream, int bufferSize = 1024,
        EncodingType encoding = EncodingType.UTF8)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var sb = new StringBuilder();
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = await stream.ReadAsync(buffer, 0, bufferSize);
            if (count == 0) break;
            sb.Append(buffer.ToText(encoding).ToCharArray(), 0, count);
        }

        var text = sb.ToString();
        return text;
    }

    public static async IAsyncEnumerable<string> ReadBlockAsync(this Stream stream, int bufferSize = 1024,
        EncodingType encoding = EncodingType.UTF8)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = await stream.ReadAsync(buffer, 0, bufferSize);
            if (count == 0) break;
            yield return buffer.ToText(0, count, encoding);
        }
    }

    public static async void ReadBlockAsync(this Stream stream, Action<string> action, int bufferSize = 1024,
        EncodingType encoding = EncodingType.UTF8)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (action == null) throw new ArgumentNullException(nameof(action));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = await stream.ReadAsync(buffer, 0, bufferSize);
            if (count == 0) break;
            var text = buffer.ToText(0, count, encoding);
            action(text);
        }
    }

    public static async void ReadBlockAsync(this Stream stream, Action<byte[]> action, int bufferSize = 1024)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (action == null) throw new ArgumentNullException(nameof(action));
        var buffer = new byte[bufferSize];
        while (true)
        {
            var count = await stream.ReadAsync(buffer, 0, bufferSize);
            if (count == 0) break;
            action(buffer);
        }
    }

    public static byte[]? ReadFixedBufferSize(this Stream stream, int bufferSize)
    {
        var buf = new byte[bufferSize];
        var offset = 0;
        do
        {
            var content = stream.Read(buf, offset, bufferSize - offset);
            if (content == 0)
                return null;
            offset += content;
        } while (offset < bufferSize);

        return buf;
    }

    public static List<string> ReadLines(this Stream stream)
    {
        var lines = new List<string>();
        using var sr = new StreamReader(stream);
        while (sr.Peek() >= 0)
        {
            var line = sr.ReadLine();
            if (line != null)
                lines.Add(line);
        }

        return lines;
    }

    public static void SaveAsFile(this Stream @this, string filePath)
    {
        @this.SaveAsFile(filePath, FileMode.Create);
    }

    public static void SaveAsFile(this Stream @this, string filePath, FileMode fileMode)
    {
        @this.SaveAsFile(filePath, fileMode, 81920);
    }

    public static void SaveAsFile(this Stream @this, string filePath, FileMode fileMode, int bufferSize)
    {
        if (@this.IsNull()) throw new ArgumentNullException(nameof(@this), "is null");
        if (filePath.IsNull()) throw new ArgumentNullException(nameof(filePath), "is null");
        if (filePath.IsEmpty()) throw new ArgumentException(nameof(filePath), $"{nameof(filePath)} empty string");
        if (bufferSize < 1)
            throw new ArgumentOutOfRangeException(nameof(bufferSize),
                $"{nameof(bufferSize)} must greater than zero");
        var dirPath = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(dirPath))
            throw new ArgumentNullException(nameof(filePath), $"{nameof(filePath)} can not find directory name");
        // ReSharper disable once AssignNullToNotNullAttribute
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
        using var fs = new FileStream(filePath, fileMode);
        @this.CopyTo(fs, bufferSize);
    }

    public static Stream SeekToBegin(this Stream stream)
    {
        if (!stream.CanSeek)
            throw new InvalidOperationException("Stream does not support seeking.");
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public static Stream SeekToEnd(this Stream stream)
    {
        if (!stream.CanSeek)
            throw new InvalidOperationException("Stream does not support seeking.");
        stream.Seek(0, SeekOrigin.End);
        return stream;
    }

    public static byte[] ToByteArray(this Stream @this)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        using var ms = new MemoryStream();
        @this.CopyTo(ms);
        return ms.ToArray();
    }

    public static byte[] ToByteArray(this StreamReader @this)
    {
        using var stream = new MemoryStream();
        @this.BaseStream.CopyTo(stream);
        var bytes = stream.ToArray();
        return bytes;
    }

    public static FileStream ToFileStream(this Stream stream, string filePath, FileMode fileMode = FileMode.Create,
        FileAccess fileAccess = FileAccess.ReadWrite)
    {
        var fs = new FileStream(filePath, fileMode, fileAccess);
        stream.CopyTo(fs);
        return fs;
    }

    public static MemoryStream ToMemoryStream(this Stream stream)
    {
        var ret = new MemoryStream();
        var buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            ret.Write(buffer, 0, bytesRead);
        ret.Position = 0;
        return ret;
    }

    public static string ToText(this Stream @this)
    {
        using var sr = new StreamReader(@this, Encoding.UTF8);
        return sr.ReadToEnd();
    }

    public static string ToText(this Stream @this, Encoding encoding)
    {
        using var sr = new StreamReader(@this, encoding);
        return sr.ReadToEnd();
    }

    public static string ToText(this Stream @this, long position)
    {
        @this.Position = position;

        using var sr = new StreamReader(@this, Encoding.UTF8);
        return sr.ReadToEnd();
    }

    public static string ToText(this Stream @this, Encoding encoding, long position)
    {
        @this.Position = position;

        using var sr = new StreamReader(@this, encoding);
        return sr.ReadToEnd();
    }

    public static void Write(this Stream stream, byte[] bytes)
    {
        stream.Write(bytes, 0, bytes.Length);
    }

    public static void WriteFile(this Stream stream, string filePath, FileMode fileMode = FileMode.Create)
    {
        using var fileStream = new FileStream(filePath, fileMode, FileAccess.Write);
        stream.CopyTo(fileStream);
    }
}