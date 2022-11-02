// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using Beyond.Extensions.StreamExtended;

namespace Beyond.Extensions.FileSignature;

// Site: https://www.filesignatures.net/
public static class FileSignatureExtensions
{
    private static readonly Dictionary<string, List<byte[]>> FileSignature = new()
    {
        {
            ".jpeg", new List<byte[]>
            {
                new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE2},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE3}
            }
        },
        {
            ".jpg", new List<byte[]>
            {
                new byte[] {0xFF, 0xD8, 0xFF, 0xE0},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE1},
                new byte[] {0xFF, 0xD8, 0xFF, 0xE8}
            }
        },
        {
            ".png", new List<byte[]>
            {
                new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A},
                new byte[] {0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D}
            }
        }
    };

    public static string GetMimeType(this IEnumerable<byte> byteArray)
    {
        return byteArray.IsJpeg() ? "image/jpeg" : string.Empty;
    }

    public static bool IsJpeg(this Stream stream)
    {
        return stream.ToByteArray().IsJpeg();
    }

    private static bool IsJpeg(this IEnumerable<byte> byteArray)
    {
        var jpegSignatures = FileSignature[".jpeg"];
        var jpgSignatures = FileSignature[".jpg"];
        var headerBytes = byteArray.Take(jpegSignatures.Max(m => m.Length));
        return jpegSignatures.Any(signature =>
                   headerBytes.Take(signature.Length).SequenceEqual(signature))
               || jpgSignatures.Any(signature =>
                   headerBytes.Take(signature.Length).SequenceEqual(signature));
    }
}