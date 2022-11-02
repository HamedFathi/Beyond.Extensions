﻿// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Beyond.Extensions.ByteArrayExtended;
using Beyond.Extensions.Enums;
using Beyond.Extensions.StreamExtended;
using Beyond.Extensions.StringExtended;

namespace Beyond.Extensions.Cryptography;

public static partial class CryptoExtensions
{
    public static byte[] DecryptAES(this byte[] data, byte[] key, byte[] iv, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = cipherMode;
        aes.Padding = paddingMode;
        using var cryptoTransform = aes.CreateDecryptor();
        return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
    }

    public static Stream DecryptAESToStream(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return DecryptAES(data, key, iv, cipherMode, paddingMode).ToStream();
    }

    public static string DecryptAESToString(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return DecryptAES(data, key, iv, cipherMode, paddingMode).ToText();
    }

    public static byte[] DecryptRijndael(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        using var aes = Rijndael.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = cipherMode;
        aes.Padding = paddingMode;
        using var cryptoTransform = aes.CreateDecryptor();
        return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
    }

    public static Stream DecryptRijndaelToStream(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return DecryptRijndael(data, key, iv, cipherMode, paddingMode).ToStream();
    }

    public static string DecryptRijndaelToString(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return DecryptRijndael(data, key, iv, cipherMode, paddingMode).ToText();
    }

    public static byte[] DecryptRSA(this byte[] data, RSAParameters privateKey,
        RSAEncryptionPaddingType padding = RSAEncryptionPaddingType.OaepSHA256)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(privateKey);
        var result = padding switch
        {
            RSAEncryptionPaddingType.Pkcs1 => rsa.Decrypt(data,
                RSAEncryptionPadding.Pkcs1),
            RSAEncryptionPaddingType.OaepSHA1 => rsa.Decrypt(data,
                RSAEncryptionPadding.OaepSHA1),
            RSAEncryptionPaddingType.OaepSHA256 => rsa.Decrypt(data,
                RSAEncryptionPadding.OaepSHA256),
            RSAEncryptionPaddingType.OaepSHA384 => rsa.Decrypt(data,
                RSAEncryptionPadding.OaepSHA384),
            RSAEncryptionPaddingType.OaepSHA512 => rsa.Decrypt(data,
                RSAEncryptionPadding.OaepSHA512),
            _ => throw new ArgumentOutOfRangeException(nameof(padding), padding, null)
        };

        return result;
    }

    public static Stream DecryptRSAToStream(this byte[] data, RSAParameters privateKey,
        RSAEncryptionPaddingType padding = RSAEncryptionPaddingType.OaepSHA256)
    {
        return DecryptRSA(data, privateKey, padding).ToStream();
    }

    public static string DecryptRSAToString(this byte[] data, RSAParameters privateKey,
        RSAEncryptionPaddingType padding = RSAEncryptionPaddingType.OaepSHA256)
    {
        return DecryptRSA(data, privateKey, padding).ToText();
    }

    public static byte[] DecryptTripleDES(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        var tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = key;
        tdes.Mode = cipherMode;
        tdes.Padding = paddingMode;
        tdes.IV = iv;
        return tdes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
    }

    public static Stream DecryptTripleDESToStream(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return DecryptTripleDES(data, key, iv, cipherMode, paddingMode).ToStream();
    }

    public static string DecryptTripleDESToString(this byte[] data, byte[] key, byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return DecryptTripleDES(data, key, iv, cipherMode, paddingMode).ToText();
    }

    public static byte[] EncryptAES(this byte[] data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        using var aes = Aes.Create();
        aes.Mode = cipherMode;
        aes.Padding = paddingMode;
        aes.Key = key;
        aes.GenerateIV();
        using var cryptoTransform = aes.CreateEncryptor();
        iv = aes.IV;
        return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
    }

    public static byte[] EncryptAES(this string data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return EncryptAES(data.ToByteArray(), key, out iv, cipherMode, paddingMode);
    }

    public static byte[] EncryptAES(this Stream data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return EncryptAES(data.ToByteArray(), key, out iv, cipherMode, paddingMode);
    }

    public static byte[] EncryptRijndael(this byte[] data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        using var aes = Rijndael.Create();
        aes.Mode = cipherMode;
        aes.Padding = paddingMode;
        aes.Key = key;
        aes.GenerateIV();
        using var cryptoTransform = aes.CreateEncryptor();
        iv = aes.IV;
        return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
    }

    public static byte[] EncryptRijndael(this Stream data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return EncryptRijndael(data.ToByteArray(), key, out iv, cipherMode, paddingMode);
    }

    public static byte[] EncryptRijndael(this string data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return EncryptRijndael(data.ToByteArray(), key, out iv, cipherMode, paddingMode);
    }

    public static byte[] EncryptRSA(this byte[] data, RSAParameters publicKey,
        RSAEncryptionPaddingType padding = RSAEncryptionPaddingType.OaepSHA256)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(publicKey);
        var result = padding switch
        {
            RSAEncryptionPaddingType.Pkcs1 => rsa.Encrypt(data,
                RSAEncryptionPadding.Pkcs1),
            RSAEncryptionPaddingType.OaepSHA1 => rsa.Encrypt(data,
                RSAEncryptionPadding.OaepSHA1),
            RSAEncryptionPaddingType.OaepSHA256 => rsa.Encrypt(data,
                RSAEncryptionPadding.OaepSHA256),
            RSAEncryptionPaddingType.OaepSHA384 => rsa.Encrypt(data,
                RSAEncryptionPadding.OaepSHA384),
            RSAEncryptionPaddingType.OaepSHA512 => rsa.Encrypt(data,
                RSAEncryptionPadding.OaepSHA512),
            _ => throw new ArgumentOutOfRangeException(nameof(padding), padding, null)
        };
        return result;
    }

    public static byte[] EncryptRSA(this Stream data, RSAParameters publicKey,
        RSAEncryptionPaddingType padding = RSAEncryptionPaddingType.OaepSHA256)
    {
        return EncryptRSA(data.ToByteArray(), publicKey, padding);
    }

    public static byte[] EncryptRSA(this string data, RSAParameters publicKey,
        RSAEncryptionPaddingType padding = RSAEncryptionPaddingType.OaepSHA256)
    {
        return EncryptRSA(data.ToByteArray(), publicKey, padding);
    }

    public static byte[] EncryptTripleDES(this byte[] data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        var tdes = new TripleDESCryptoServiceProvider();
        tdes.Key = key;
        tdes.Mode = cipherMode;
        tdes.Padding = paddingMode;
        tdes.GenerateIV();
        iv = tdes.IV;
        return tdes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
    }

    public static byte[] EncryptTripleDES(this Stream data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return EncryptTripleDES(data.ToByteArray(), key, out iv, cipherMode, paddingMode);
    }

    public static byte[] EncryptTripleDES(this string data, byte[] key, out byte[] iv,
        CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        return EncryptTripleDES(data.ToByteArray(), key, out iv, cipherMode, paddingMode);
    }

    public static byte[] SignData(this byte[] data, RSAParameters privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(privateKey);
        byte[] hash;
        using (var sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(data);
        }

        var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
        rsaFormatter.SetHashAlgorithm("SHA256");
        return rsaFormatter.CreateSignature(hash);
    }

    public static byte[] SignData(this string data, RSAParameters privateKey)
    {
        return SignData(data.ToByteArray(), privateKey);
    }

    public static byte[] SignData(this Stream data, RSAParameters privateKey)
    {
        return SignData(data.ToByteArray(), privateKey);
    }

    public static string ToMD5(this string data)
    {
        return ToMD5(data.ToByteArray());
    }

    public static string ToMD5(this Stream data)
    {
        return ToMD5(data.ToByteArray());
    }

    public static string ToMD5(this byte[] data)
    {
        using var md5Hash = MD5.Create();
        var bytes = md5Hash.ComputeHash(data);
        var builder = new StringBuilder();
        foreach (var t in bytes) builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static string ToSHA1(this string data)
    {
        return ToSHA1(data.ToByteArray());
    }

    public static string ToSHA1(this Stream data)
    {
        return ToSHA1(data.ToByteArray());
    }

    public static string ToSHA1(this byte[] data)
    {
        using var sha1Hash = SHA1.Create();
        var bytes = sha1Hash.ComputeHash(data);
        var builder = new StringBuilder();
        foreach (var t in bytes) builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static string ToSHA256(this string data)
    {
        return ToSHA256(data.ToByteArray());
    }

    public static string ToSHA256(this Stream data)
    {
        return ToSHA256(data.ToByteArray());
    }

    public static string ToSHA256(this byte[] data)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(data);
        var builder = new StringBuilder();
        foreach (var t in bytes) builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static string ToSHA384(this string data)
    {
        return ToSHA384(data.ToByteArray());
    }

    public static string ToSHA384(this Stream data)
    {
        return ToSHA384(data.ToByteArray());
    }

    public static string ToSHA384(this byte[] data)
    {
        using var sha384Hash = SHA384.Create();
        var bytes = sha384Hash.ComputeHash(data);
        var builder = new StringBuilder();
        foreach (var t in bytes) builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static string ToSHA512(this string data)
    {
        return ToSHA512(data.ToByteArray());
    }

    public static string ToSHA512(this Stream data)
    {
        return ToSHA512(data.ToByteArray());
    }

    public static string ToSHA512(this byte[] data)
    {
        using var sha512Hash = SHA512.Create();
        var bytes = sha512Hash.ComputeHash(data);
        var builder = new StringBuilder();
        foreach (var t in bytes) builder.Append(t.ToString("x2"));

        return builder.ToString();
    }

    public static bool VerifySignedData(this byte[] data, byte[] signature, RSAParameters publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportParameters(publicKey);
        byte[] hash;
        using (var sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(data);
        }

        var rsaFormatter = new RSAPKCS1SignatureDeformatter(rsa);
        rsaFormatter.SetHashAlgorithm("SHA256");
        return rsaFormatter.VerifySignature(hash, signature);
    }

    public static bool VerifySignedData(this string data, byte[] signature, RSAParameters publicKey)
    {
        return VerifySignedData(data.ToByteArray(), signature, publicKey);
    }

    public static bool VerifySignedData(this Stream data, byte[] signature, RSAParameters publicKey)
    {
        return VerifySignedData(data.ToByteArray(), signature, publicKey);
    }
}