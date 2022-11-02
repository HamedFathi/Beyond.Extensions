// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

using Beyond.Extensions.ByteArrayExtended;
using Beyond.Extensions.Enums;
using Beyond.Extensions.StreamExtended;
using Beyond.Extensions.StringExtended;

namespace Beyond.Extensions.Cryptography;

public static partial class CryptoExtensions
{
    public static byte[] ToPBKDF2(this string data, string salt = "",
        HashAlgorithmMode hashAlgorithm = HashAlgorithmMode.SHA256, int hashSize = 24, int iterations = 10000)
    {
        byte[] _salt;
        if (string.IsNullOrEmpty(salt))
        {
            var provider = new RNGCryptoServiceProvider();
            _salt = new byte[24];
            provider.GetBytes(_salt);
        }
        else
        {
            _salt = salt.ToByteArray();
        }

        var hashAlg = hashAlgorithm switch
        {
            HashAlgorithmMode.MD5 => HashAlgorithmName.MD5,
            HashAlgorithmMode.SHA1 => HashAlgorithmName.SHA1,
            HashAlgorithmMode.SHA256 => HashAlgorithmName.SHA256,
            HashAlgorithmMode.SHA384 => HashAlgorithmName.SHA384,
            HashAlgorithmMode.SHA512 => HashAlgorithmName.SHA512,
            _ => HashAlgorithmName.SHA256
        };
        using var deriveBytes = new Rfc2898DeriveBytes(data, _salt, iterations, hashAlg);
        return deriveBytes.GetBytes(hashSize);
    }

    public static byte[] ToPBKDF2(this Stream data, string salt = "",
        HashAlgorithmMode HashAlgorithmMode = HashAlgorithmMode.SHA256, int hashSize = 24, int iterations = 10000)
    {
        return ToPBKDF2(data.ToText(), salt, HashAlgorithmMode, hashSize, iterations);
    }

    public static byte[] ToPBKDF2(this byte[] data, string salt = "",
        HashAlgorithmMode HashAlgorithmMode = HashAlgorithmMode.SHA256, int hashSize = 24, int iterations = 10000)
    {
        return ToPBKDF2(data.ToText(), salt, HashAlgorithmMode, hashSize, iterations);
    }
}