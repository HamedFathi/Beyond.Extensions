// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

using Beyond.Extensions.Internals.Crc;
using System.Security.Cryptography.X509Certificates;

namespace Beyond.Extensions.Cryptography;

public static class CryptoExtensions
{
    public static string AesDecrypt(this byte[] cipherText, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }

    public static string AesDecrypt(this string cipherText, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return AesDecryptWithBase64Key(cipherText, key, iv);
    }

    public static byte[] AesDecryptAsBytes(this byte[] cipherText, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

        var decryptedBytes = new byte[cipherText.Length];
        var bytesRead = cs.Read(decryptedBytes, 0, decryptedBytes.Length);

        return decryptedBytes.Take(bytesRead).ToArray();
    }

    public static byte[] AesDecryptAsBytes(this byte[] cipherText, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return AesDecryptAsBytesWithBase64Key(cipherText, key, iv);
    }

    public static byte[] AesDecryptAsBytesWithBase64Key(this byte[] cipherText, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return AesDecryptAsBytes(cipherText, key, iv);
    }

    public static Stream AesDecryptAsStream(this byte[] cipherText, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var ms = new MemoryStream(cipherText);
        var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

        var resultStream = new MemoryStream();
        cs.CopyTo(resultStream);
        resultStream.Position = 0;

        return resultStream;
    }

    public static Stream AesDecryptAsStream(this Stream cipherTextStream, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return AesDecryptAsStreamWithBase64Key(cipherTextStream, key, iv);
    }

    public static Stream AesDecryptAsStreamWithBase64Key(this Stream cipherTextStream, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        var cipherText = new byte[cipherTextStream.Length];
        var _ = cipherTextStream.Read(cipherText, 0, cipherText.Length);
        return AesDecryptAsStream(cipherText, key, iv);
    }

    public static string AesDecryptWithBase64Key(this string cipherText, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        var cipherBytes = Convert.FromBase64String(cipherText);
        return AesDecrypt(cipherBytes, key, iv);
    }

    public static byte[] AesEncrypt(this string plaintext, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plaintext);
        }

        return ms.ToArray();
    }

    public static byte[] AesEncrypt(this byte[] data, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(data, 0, data.Length);
        }

        return ms.ToArray();
    }

    public static Stream AesEncrypt(this Stream stream, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            stream.CopyTo(cs);
        }

        ms.Position = 0;
        return ms;
    }

    public static byte[] AesEncrypt(this string plaintext, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return AesEncryptWithBase64Key(plaintext, key, iv);
    }

    public static byte[] AesEncrypt(this byte[] data, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return AesEncryptWithBase64Key(data, key, iv);
    }

    public static Stream AesEncrypt(this Stream stream, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return AesEncryptWithBase64Key(stream, key, iv);
    }

    public static byte[] AesEncryptWithBase64Key(this string plaintext, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return AesEncrypt(plaintext, key, iv);
    }

    public static byte[] AesEncryptWithBase64Key(this byte[] data, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return AesEncrypt(data, key, iv);
    }

    public static Stream AesEncryptWithBase64Key(this Stream stream, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return AesEncrypt(stream, key, iv);
    }

    public static uint CalculateCRC32Hash(this string input)
    {
        return Crc32.Compute(Encoding.UTF8.GetBytes(input));
    }

    public static uint CalculateCRC32Hash(this byte[] input)
    {
        return Crc32.Compute(input);
    }

    public static uint CalculateCRC32Hash(this Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Crc32.Compute(ms.ToArray());
    }

    public static ulong CalculateCRC64Hash(this string input)
    {
        return Crc64.Compute(Encoding.UTF8.GetBytes(input));
    }

    public static ulong CalculateCRC64Hash(this byte[] input)
    {
        return Crc64.Compute(input);
    }

    public static ulong CalculateCRC64Hash(this Stream stream)
    {
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Crc64.Compute(ms.ToArray());
    }

    public static string CalculateMD5Hash(this string input)
    {
        using var md5 = MD5.Create();
        return GetHash(md5, input);
    }

    public static string CalculateMD5Hash(this byte[] input)
    {
        using var md5 = MD5.Create();
        return GetHash(md5, input);
    }

    public static string CalculateMD5Hash(this Stream stream)
    {
        using var md5 = MD5.Create();
        return GetHash(md5, stream);
    }

    public static byte[] CalculatePBKDF2Hash(this Stream stream, byte[]? salt = null, int iterations = 1000, int hashByteSize = 20)
    {
        salt ??= GenerateRandomSalt();
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var data = ms.ToArray();
        var dataAsString = Convert.ToBase64String(data);
        using var pbkdf2 = new Rfc2898DeriveBytes(dataAsString, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(hashByteSize);
    }

    public static byte[] CalculatePBKDF2Hash(this byte[] data, byte[]? salt = null, int iterations = 1000, int hashByteSize = 20)
    {
        salt ??= GenerateRandomSalt();
        var dataAsString = Convert.ToBase64String(data);
        using var pbkdf2 = new Rfc2898DeriveBytes(dataAsString, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(hashByteSize);
    }

    public static byte[] CalculatePBKDF2Hash(this string password, byte[]? salt = null, int iterations = 1000, int hashByteSize = 20)
    {
        salt ??= GenerateRandomSalt();
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(hashByteSize);
    }

    public static string CalculateSHA1Hash(this string input)
    {
        using var sha1 = SHA1.Create();
        return GetHash(sha1, Encoding.UTF8.GetBytes(input));
    }

    public static string CalculateSHA1Hash(this byte[] input)
    {
        using var sha1 = SHA1.Create();
        return GetHash(sha1, input);
    }

    public static string CalculateSHA1Hash(this Stream stream)
    {
        using var sha1 = SHA1.Create();
        return GetHash(sha1, stream);
    }

    public static string CalculateSHA256Hash(this string input)
    {
        using var sha256 = SHA256.Create();
        return GetHash(sha256, Encoding.UTF8.GetBytes(input));
    }

    public static string CalculateSHA256Hash(this byte[] input)
    {
        using var sha256 = SHA256.Create();
        return GetHash(sha256, input);
    }

    public static string CalculateSHA256Hash(this Stream stream)
    {
        using var sha256 = SHA256.Create();
        return GetHash(sha256, stream);
    }

    public static string CalculateSHA384Hash(this string input)
    {
        using var sha384 = SHA384.Create();
        return GetHash(sha384, Encoding.UTF8.GetBytes(input));
    }

    public static string CalculateSHA384Hash(this byte[] input)
    {
        using var sha384 = SHA384.Create();
        return GetHash(sha384, input);
    }

    public static string CalculateSHA384Hash(this Stream stream)
    {
        using var sha384 = SHA384.Create();
        return GetHash(sha384, stream);
    }

    public static string CalculateSHA512Hash(this string input)
    {
        using var sha512 = SHA512.Create();
        return GetHash(sha512, Encoding.UTF8.GetBytes(input));
    }

    public static string CalculateSHA512Hash(this byte[] input)
    {
        using var sha512 = SHA512.Create();
        return GetHash(sha512, input);
    }

    public static string CalculateSHA512Hash(this Stream stream)
    {
        using var sha512 = SHA512.Create();
        return GetHash(sha512, stream);
    }

    public static X509Certificate2 CreateSelfSignedX509Certificate(this string subjectName)
    {
        var distinguishedName = new X500DistinguishedName($"CN={subjectName}");

        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5));
    }

    public static byte[] GenerateDigitalSignature(this byte[] data, RSAParameters privateKey)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(privateKey);
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public static string RSADecrypt(this string cipherText, RSAParameters? rsaParams = null)
    {
        var bytesToDecrypt = Convert.FromBase64String(cipherText);

        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParams ?? GenerateRSAKeyPair());

        var decryptedBytes = rsa.Decrypt(bytesToDecrypt, false);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public static byte[] RSADecrypt(this byte[] bytesToDecrypt, RSAParameters? rsaParams = null)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParams ?? GenerateRSAKeyPair());

        return rsa.Decrypt(bytesToDecrypt, false);
    }

    public static string RSAEncrypt(this string plainText, RSAParameters? rsaParams = null)
    {
        var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);

        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParams ?? GenerateRSAKeyPair());

        var encryptedBytes = rsa.Encrypt(bytesToEncrypt, false);
        return Convert.ToBase64String(encryptedBytes);
    }

    public static byte[] RSAEncrypt(this byte[] bytesToEncrypt, RSAParameters? rsaParams = null)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaParams ?? GenerateRSAKeyPair());

        return rsa.Encrypt(bytesToEncrypt, false);
    }

    public static byte[] SaveX509CertificateToBytes(this X509Certificate2 certificate, string password, X509ContentType contentType = X509ContentType.Pfx)
    {
        return certificate.Export(contentType, password);
    }

    public static Stream SaveX509CertificateToStream(this X509Certificate2 certificate, string password, X509ContentType contentType = X509ContentType.Pfx)
    {
        var bytes = certificate.Export(contentType, password);
        return new MemoryStream(bytes);
    }

    public static string TripleDesDecrypt(this byte[] cipherText, byte[] key, byte[] iv)
    {
        using var tripleDES = TripleDES.Create();
        tripleDES.Key = key;
        tripleDES.IV = iv;

        using var decryptor = tripleDES.CreateDecryptor(tripleDES.Key, tripleDES.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }

    public static string TripleDesDecrypt(this string cipherText, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return TripleDesDecryptWithBase64Key(cipherText, key, iv);
    }

    public static byte[] TripleDesDecryptAsBytes(this byte[] cipherText, byte[] key, byte[] iv)
    {
        using var tripleDES = TripleDES.Create();
        tripleDES.Key = key;
        tripleDES.IV = iv;

        using var decryptor = tripleDES.CreateDecryptor(tripleDES.Key, tripleDES.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

        var decryptedBytes = new byte[cipherText.Length];
        var bytesRead = cs.Read(decryptedBytes, 0, decryptedBytes.Length);

        return decryptedBytes.Take(bytesRead).ToArray();
    }

    public static byte[] TripleDesDecryptAsBytes(this byte[] cipherText, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return TripleDesDecryptAsBytesWithBase64Key(cipherText, key, iv);
    }

    public static byte[] TripleDesDecryptAsBytesWithBase64Key(this byte[] cipherText, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return TripleDesDecryptAsBytes(cipherText, key, iv);
    }

    public static Stream TripleDesDecryptAsStream(this byte[] cipherText, byte[] key, byte[] iv)
    {
        using var tripleDES = TripleDES.Create();
        tripleDES.Key = key;
        tripleDES.IV = iv;

        using var decryptor = tripleDES.CreateDecryptor(tripleDES.Key, tripleDES.IV);
        var ms = new MemoryStream(cipherText);
        var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

        var resultStream = new MemoryStream();
        cs.CopyTo(resultStream);
        resultStream.Position = 0;

        return resultStream;
    }

    public static Stream TripleDesDecryptAsStream(this Stream cipherTextStream, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return TripleDesDecryptAsStreamWithBase64Key(cipherTextStream, key, iv);
    }

    public static Stream TripleDesDecryptAsStreamWithBase64Key(this Stream cipherTextStream, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        var cipherText = new byte[cipherTextStream.Length];
        var _ = cipherTextStream.Read(cipherText, 0, cipherText.Length);
        return TripleDesDecryptAsStream(cipherText, key, iv);
    }

    public static string TripleDesDecryptWithBase64Key(this string cipherText, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        var cipherBytes = Convert.FromBase64String(cipherText);
        return TripleDesDecrypt(cipherBytes, key, iv);
    }

    public static byte[] TripleDesEncrypt(this string plaintext, byte[] key, byte[] iv)
    {
        using var tripleDES = TripleDES.Create();
        tripleDES.Key = key;
        tripleDES.IV = iv;

        using var encryptor = tripleDES.CreateEncryptor(tripleDES.Key, tripleDES.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plaintext);
        }

        return ms.ToArray();
    }

    public static byte[] TripleDesEncrypt(this byte[] data, byte[] key, byte[] iv)
    {
        using var tripleDES = TripleDES.Create();
        tripleDES.Key = key;
        tripleDES.IV = iv;

        using var encryptor = tripleDES.CreateEncryptor(tripleDES.Key, tripleDES.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(data, 0, data.Length);
        }

        return ms.ToArray();
    }

    public static Stream TripleDesEncrypt(this Stream stream, byte[] key, byte[] iv)
    {
        using var tripleDES = TripleDES.Create();
        tripleDES.Key = key;
        tripleDES.IV = iv;

        using var encryptor = tripleDES.CreateEncryptor(tripleDES.Key, tripleDES.IV);
        var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            stream.CopyTo(cs);
        }

        ms.Position = 0;
        return ms;
    }

    public static byte[] TripleDesEncrypt(this string plaintext, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return TripleDesEncryptWithBase64Key(plaintext, key, iv);
    }

    public static byte[] TripleDesEncrypt(this byte[] data, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return TripleDesEncryptWithBase64Key(data, key, iv);
    }

    public static Stream TripleDesEncrypt(this Stream stream, string keyText, string ivText)
    {
        var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyText));
        var iv = Convert.ToBase64String(Encoding.UTF8.GetBytes(ivText));
        return TripleDesEncryptWithBase64Key(stream, key, iv);
    }

    public static byte[] TripleDesEncryptWithBase64Key(this string plaintext, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return TripleDesEncrypt(plaintext, key, iv);
    }

    public static byte[] TripleDesEncryptWithBase64Key(this byte[] data, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return TripleDesEncrypt(data, key, iv);
    }

    public static Stream TripleDesEncryptWithBase64Key(this Stream stream, string base64Key, string base64Iv)
    {
        var key = Convert.FromBase64String(base64Key);
        var iv = Convert.FromBase64String(base64Iv);
        return TripleDesEncrypt(stream, key, iv);
    }

    public static bool VerifyDigitalSignature(this byte[] data, byte[] signature, RSAParameters publicKey)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(publicKey);
        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public static bool VerifyX509Certificate(this X509Certificate2 certificate)
    {
        var chain = new X509Chain
        {
            ChainPolicy =
            {
                RevocationMode = X509RevocationMode.Online,
                RevocationFlag = X509RevocationFlag.EntireChain
            }
        };

        return chain.Build(certificate);
    }

    private static byte[] GenerateRandomSalt(int saltSize = 16)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[saltSize];
        rng.GetBytes(salt);
        return salt;
    }

    private static RSAParameters GenerateRSAKeyPair()
    {
        using var rsa = new RSACryptoServiceProvider(2048);
        return rsa.ExportParameters(true);
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(data).Replace("-", string.Empty);
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, byte[] input)
    {
        var data = hashAlgorithm.ComputeHash(input);
        return BitConverter.ToString(data).Replace("-", string.Empty);
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, Stream stream)
    {
        var data = hashAlgorithm.ComputeHash(stream);
        return BitConverter.ToString(data).Replace("-", string.Empty);
    }
}