﻿// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

using Beyond.Extensions.CharExtended;
using Beyond.Extensions.Enums;
using Beyond.Extensions.Internals.Base62;
using Beyond.Extensions.Internals.Pluralizer;
using Beyond.Extensions.Types;

// ReSharper disable InconsistentNaming

// ReSharper disable CommentTypo

namespace Beyond.Extensions.StringExtended;

public static class StringExtensions
{
    private static readonly Pluralizer PluralizeHelper = new();

    private static readonly Regex TabOrWhiteSpaceRegex =
        new(@"(\s*\\\$tb(\d+)\s*)|(\s*\\\$ws(\d+)\s*)", RegexOptions.Compiled);

    public static DirectoryInfo AsDirectoryInfo(this string @this)
    {
        if (@this.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(@this), $"{nameof(@this)} path is null or empty");
        return new DirectoryInfo(@this);
    }

    public static FileStream AsFileStream(this string @this, FileMode fileMode, FileAccess fileAccess,
        FileShare fileShare, int bufferSize = 8192)
    {
        if (@this.IsNull()) throw new ArgumentNullException(nameof(@this), $"{nameof(@this)} file path is null");
        return new FileStream(@this, fileMode, fileAccess, fileShare, bufferSize);
    }

    public static int CompareOrdinal(this string strA, string strB)
    {
        return string.CompareOrdinal(strA, strB);
    }

    public static int CompareOrdinal(this string strA, int indexA, string strB, int indexB, int length)
    {
        return string.CompareOrdinal(strA, indexA, strB, indexB, length);
    }

    public static string Concat(this string str0, string str1)
    {
        return string.Concat(str0, str1);
    }

    public static string Concat(this string str0, string str1, string str2)
    {
        return string.Concat(str0, str1, str2);
    }

    public static string Concat(this string str0, string str1, string str2, string str3)
    {
        return string.Concat(str0, str1, str2, str3);
    }

    public static string Concat(this IEnumerable<string> @this)
    {
        var sb = new StringBuilder();

        foreach (var s in @this) sb.Append(s);

        return sb.ToString();
    }

    public static string Concat<T>(this IEnumerable<T> source, Func<T, string> func)
    {
        var sb = new StringBuilder();
        foreach (var item in source) sb.Append(func(item));

        return sb.ToString();
    }

    public static string ConcatWith(this string @this, params string[] values)
    {
        return string.Concat(@this, string.Concat(values));
    }

    public static bool Contains(this string @this, string value, StringComparison comparisonType)
    {
        return @this.IndexOf(value, comparisonType) != -1;
    }

    public static bool ContainsAll(this string @this, params string[] values)
    {
        foreach (var value in values)
            if (@this.IndexOf(value, StringComparison.Ordinal) == -1)
                return false;
        return true;
    }

    public static bool ContainsAll(this string @this, StringComparison comparisonType, params string[] values)
    {
        foreach (var value in values)
            if (@this.IndexOf(value, comparisonType) == -1)
                return false;
        return true;
    }

    public static bool ContainsAll(this string @this, IEnumerable<string> values)
    {
        return @this.ContainsAll(values, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool ContainsAll(this string @this, IEnumerable<string> values, StringComparison comparisonType)
    {
        return values.All(s => @this.IndexOf(s, comparisonType) >= 0);
    }

    public static bool ContainsAny(this string @this, params string[] values)
    {
        foreach (var value in values)
            if (@this.IndexOf(value, StringComparison.Ordinal) != -1)
                return true;
        return false;
    }

    public static bool ContainsAny(this string @this, StringComparison comparisonType, params string[] values)
    {
        foreach (var value in values)
            if (@this.IndexOf(value, comparisonType) != -1)
                return true;
        return false;
    }

    public static bool ContainsAny(this string @this, IEnumerable<string> values, StringComparison comparisonType)
    {
        return values.Any(s => @this.IndexOf(s, comparisonType) >= 0);
    }

    public static bool ContainsAny(this string @this, IEnumerable<string> values)
    {
        return @this.ContainsAny(values, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool ContainsChar(this string str, char ch, bool ignoreCase = false)
    {
        if (ignoreCase)
            return str.ToLower().IndexOf(ch.ToLower()) != -1;

        return str.IndexOf(ch) != -1;
    }

    public static bool ContainsIgnoreCase(this string a, string b)
    {
        return a.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool ContainsRegex(this string str, string regex)
    {
        var rx = new Regex(regex, RegexOptions.Compiled);
        return rx.IsMatch(str);
    }

    public static bool ContainsRegex(this string str, Regex regex)
    {
        return regex.IsMatch(str);
    }

    public static string ConvertBreaksToCRLF(this string plainText)
    {
        return new Regex("<br/>").Replace(plainText, "\r\n");
    }

    public static string ConvertBreaksToLF(this string plainText)
    {
        return new Regex("<br/>").Replace(plainText, "\n");
    }

    public static string ConvertCRLFToBreaks(this string plainText)
    {
        return new Regex("(\r\n|\n)").Replace(plainText, "<br/>");
    }

    public static string ConvertNewLineToWhiteSpace(this string str)
    {
        return Regex.Replace(str, @"\t|\n|\r", " ").Trim();
    }

    public static string ConvertPersianNumbersToEnglish(this string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return
            input
                .Replace("\u0660", "0") //٠
                .Replace("\u06F0", "0") //۰
                .Replace("\u0661", "1") //١
                .Replace("\u06F1", "1") //۱
                .Replace("\u0662", "2") //٢
                .Replace("\u06F2", "2") //۲
                .Replace("\u0663", "3") //٣
                .Replace("\u06F3", "3") //۳
                .Replace("\u0664", "4") //٤
                .Replace("\u06F4", "4") //۴
                .Replace("\u0665", "5") //٥
                .Replace("\u06F5", "5") //۵
                .Replace("\u0666", "6") //٦
                .Replace("\u06F6", "6") //۶
                .Replace("\u0667", "7") //٧
                .Replace("\u06F7", "7") //۷
                .Replace("\u0668", "8") //٨
                .Replace("\u06F8", "8") //۸
                .Replace("\u0669", "9") //٩
                .Replace("\u06F9", "9") //۹
            ;
    }

    public static int ConvertToUtf32(this string s, int index)
    {
        return char.ConvertToUtf32(s, index);
    }

    public static bool DoesNotEndWith(this string input, string value)
    {
        return string.IsNullOrEmpty(value) || input.IsNullOrEmptyOrWhiteSpace() || !input.EndsWith(value);
    }

    public static bool DoesNotEndWith(this string input, string value, bool ignoreCase, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value) || input.IsNullOrEmptyOrWhiteSpace() ||
               !input.EndsWith(value, ignoreCase, culture);
    }

    public static bool DoesNotEndWith(this string input, string value, StringComparison comparisonType)
    {
        return string.IsNullOrEmpty(value) || input.IsNullOrEmptyOrWhiteSpace() ||
               !input.EndsWith(value, comparisonType);
    }

    public static bool DoesNotStartWith(this string input, string value)
    {
        return string.IsNullOrEmpty(value) || input.IsNullOrEmptyOrWhiteSpace() || !input.StartsWith(value);
    }

    public static bool DoesNotStartWith(this string input, string value, bool ignoreCase, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value) || input.IsNullOrEmptyOrWhiteSpace() ||
               !input.StartsWith(value, ignoreCase, culture);
    }

    public static bool DoesNotStartWith(this string input, string value, StringComparison comparisonType)
    {
        return string.IsNullOrEmpty(value) || input.IsNullOrEmptyOrWhiteSpace() ||
               !input.StartsWith(value, comparisonType);
    }

    public static bool EndsWithIgnoreCase(this string a, string b)
    {
        return a.EndsWith(b, StringComparison.OrdinalIgnoreCase);
    }

    public static bool EqualsIgnoreCase(this string a, string b)
    {
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    public static string EscapeXml(this string @this)
    {
        return @this
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
            ;
    }

    public static string Extract(this string @this, Func<char, bool> predicate)
    {
        return new string(@this.ToCharArray().Where(predicate).ToArray());
    }

    public static string ExtractLetter(this string @this)
    {
        return new string(@this.ToCharArray().Where(char.IsLetter).ToArray());
    }

    public static IEnumerable<int> FindAllIndexOf(this string str, string substr, bool ignoreCase)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentException(nameof(str));
        if (string.IsNullOrEmpty(substr)) throw new ArgumentException(nameof(substr));
        var indexes = new List<int>();
        var index = 0;
        while (
            (index =
                str.IndexOf(substr, index,
                    ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
            indexes.Add(index++);
        return indexes;
    }

    public static IEnumerable<int> FindAllIndexOf(this string text, string pattern)
    {
        var indices = new List<int>();
        foreach (Match match in Regex.Matches(text, pattern))
            indices.Add(match.Index);

        return indices;
    }

    public static IEnumerable<int> FindAllIndexOf(this string text, Regex pattern)
    {
        var indices = new List<int>();
        foreach (Match match in pattern.Matches(text))
            indices.Add(match.Index);

        return indices;
    }

    public static IEnumerable<int> FindAllIndexOf<T>(this T[] @this, Predicate<T> predicate) where T : class
    {
        var subArray = Array.FindAll(@this, predicate);
        return from T item in subArray select Array.IndexOf(@this, item);
    }

    public static string? FirstChar(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            if (input.Length >= 1)
                return input[0].ToString();
            return input;
        }

        return null;
    }

    public static string FirstCharToLower(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToLower() + input.Substring(1)
        };
    }

    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };
    }

    public static string FixPersianChars(string value)
    {
        return value
                .Replace("ي", "ی")
                .Replace("ك", "ک")
                .Replace("‍", "")
                .Replace("دِ", "د")
                .Replace("بِ", "ب")
                .Replace("زِ", "ز")
                .Replace("ذِ", "ذ")
                .Replace("ِشِ", "ش")
                .Replace("ِسِ", "س")
                .Replace("ى", "ی")
                .Replace("ة", "ه")
            ;
    }

    public static string Format(this string format, object[] args)
    {
        return string.Format(format, args);
    }

    public static long FromBase(this string input,
        string baseChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
    {
        var srcBase = baseChars.Length;
        long id = 0;
        var text = new string(input.Reverse().ToArray());
        for (var i = 0; i < text.Length; i++)
        {
            var charIndex = baseChars.IndexOf(text[i]);
            id += charIndex * (long)Math.Pow(srcBase, i);
        }

        return id;
    }

    public static string FromBase62String(this string base62Text,
        Base62CharacterSet charSet = Base62CharacterSet.Default)
    {
        var base62 = new Base62Converter(charSet);
        return base62.Decode(base62Text);
    }

    public static string FromBase64String(this string encodedValue)
    {
        return encodedValue.FromBase64String(Encoding.UTF8);
    }

    public static string FromBase64String(this string encodedValue, Encoding encoding)
    {
        var bytes = Convert.FromBase64String(encodedValue);
        return encoding.GetString(bytes);
    }

    public static Guid FromUrlFriendlyString(this string str)
    {
        str = str.Replace('_', '/').Replace('-', '+');
        var byteArray = Convert.FromBase64String(str + "==");
        return new Guid(byteArray);
    }

    public static string GetAfter(this string @this, string value)
    {
        if (@this.IndexOf(value, StringComparison.Ordinal) == -1) return "";
        return @this.Substring(@this.IndexOf(value, StringComparison.Ordinal) + value.Length);
    }

    public static string GetBefore(this string @this, string value)
    {
        if (@this.IndexOf(value, StringComparison.Ordinal) == -1) return "";
        return @this.Substring(0, @this.IndexOf(value, StringComparison.Ordinal));
    }

    public static string GetBetween(this string @this, string before, string after)
    {
        var beforeStartIndex = @this.IndexOf(before, StringComparison.Ordinal);
        var startIndex = beforeStartIndex + before.Length;
        var afterStartIndex = @this.IndexOf(after, startIndex, StringComparison.Ordinal);

        if (beforeStartIndex == -1 || afterStartIndex == -1) return "";

        return @this.Substring(startIndex, afterStartIndex - startIndex);
    }

    public static Encoding GetEncoding(string filePath)
    {
        // Read the BOM
        var bom = new byte[4];
        using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            var _ = file.Read(bom, 0, 4);
        }

        // Analyze the BOM
        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
            return new UTF32Encoding(true, true); //UTF-32BE

        // We actually have no idea what the encoding is if we reach this point, so you may wish to
        // return null instead of defaulting to ASCII
        return Encoding.ASCII;
    }

    public static MatchCollection GetMatches(this string value, string regexPattern)
    {
        return GetMatches(value, regexPattern, RegexOptions.None);
    }

    public static MatchCollection GetMatches(this string value, string regexPattern, RegexOptions options)
    {
        return Regex.Matches(value, regexPattern, options);
    }

    public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern)
    {
        return GetMatchingValues(value, regexPattern, RegexOptions.None);
    }

    public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern, RegexOptions options)
    {
        foreach (Match match in GetMatches(value, regexPattern, options))
            if (match.Success)
                yield return match.Value;
    }

    public static IEnumerable<string> GetPathParts(this string path)
    {
        return path.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static Position GetPosition(this string text, int position)
    {
        var line = 1;
        var col = 0;
        for (var i = 0; i <= position; i++)
            if (text[i] == '\n')
            {
                line++;
                col = 0;
            }
            else
            {
                col++;
            }

        return new Position(text[position].ToString(), line, col - 1, position);
    }

    public static string GetRegexGroupValue(this string text, string regex, string groupName)
    {
        var match = new Regex(regex).Match(text);
        return match.Success ? match.Groups[groupName].Value : string.Empty;
    }

    public static string GetRegexGroupValue(this string text, Regex regex, string groupName)
    {
        var match = regex.Match(text);
        return match.Success ? match.Groups[groupName].Value : string.Empty;
    }

    public static string? GetValueOrDefault(this string? input, string? defaultValue = default,
        bool whitespaceAsEmpty = true)
    {
        return whitespaceAsEmpty ? string.IsNullOrWhiteSpace(input) ? defaultValue : input :
            string.IsNullOrEmpty(input) ? defaultValue : input;
    }

    public static string GZipCompress(this string text)
    {
        var buffer = Encoding.UTF8.GetBytes(text);
        var ms = new MemoryStream();
        using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            zip.Write(buffer, 0, buffer.Length);
        }

        ms.Position = 0;

        var compressed = new byte[ms.Length];
        // ReSharper disable once MustUseReturnValue
        ms.Read(compressed, 0, compressed.Length);

        var gzBuffer = new byte[compressed.Length + 4];
        Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
        return Convert.ToBase64String(gzBuffer);
    }

    public static string GZipDecompress(this string compressedText)
    {
        var gzBuffer = Convert.FromBase64String(compressedText);
        using var ms = new MemoryStream();
        var msgLength = BitConverter.ToInt32(gzBuffer, 0);
        ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

        var buffer = new byte[msgLength];

        ms.Position = 0;
        using (var zip = new GZipStream(ms, CompressionMode.Decompress))
        {
            // ReSharper disable once MustUseReturnValue
            zip.Read(buffer, 0, buffer.Length);
        }

        return Encoding.UTF8.GetString(buffer);
    }

    public static bool HasMultipleInstancesOf(this string input, char charToFind)
    {
        if (string.IsNullOrEmpty(input) || input.Length == 0 || input.IndexOf(charToFind) == 0)
            return false;

        if (input.IndexOf(charToFind) != input.LastIndexOf(charToFind))
            return true;

        return false;
    }

    public static byte[] HexStringToByteArray(this string hexString)
    {
        var stringLength = hexString.Length;
        var bytes = new byte[stringLength / 2];
        for (var i = 0; i < stringLength; i += 2) bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        return bytes;
    }

    public static string HtmlDecode(this string s)
    {
        return HttpUtility.HtmlDecode(s);
    }

    public static void HtmlDecode(this string s, TextWriter output)
    {
        HttpUtility.HtmlDecode(s, output);
    }

    public static string HtmlEncode(this string s)
    {
        return HttpUtility.HtmlEncode(s);
    }

    public static void HtmlEncode(this string s, TextWriter output)
    {
        HttpUtility.HtmlEncode(s, output);
    }

    public static string IfEmpty(this string value, string defaultValue)
    {
        return value.IsNotEmpty() ? value : defaultValue;
    }

    public static string IfNotNullOrEmptyElse(this string input, Func<string> alternateAction)
    {
        return string.IsNullOrEmpty(input) ? input : alternateAction();
    }

    public static string IfNotNullOrWhiteSpaceElse(this string input, Func<string> alternateAction)
    {
        return string.IsNullOrWhiteSpace(input) ? input : alternateAction();
    }

    public static string IfNullOrEmptyElse(this string input, string nullAlternateValue)
    {
        return !string.IsNullOrEmpty(input) ? input : nullAlternateValue;
    }

    public static string IfNullOrEmptyElse(this string input, Func<string> nullAlternateAction)
    {
        return !string.IsNullOrEmpty(input) ? input : nullAlternateAction();
    }

    public static string IfNullOrWhiteSpaceElse(this string input, string nullAlternateValue)
    {
        return !string.IsNullOrWhiteSpace(input) ? input : nullAlternateValue;
    }

    public static string IfNullOrWhiteSpaceElse(this string input, Func<string> nullAlternateAction)
    {
        return !string.IsNullOrWhiteSpace(input) ? input : nullAlternateAction();
    }

    public static bool IgnoreCaseEquals(this string @this, string value)
    {
        return @this.Equals(value, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool In(this string @this, params string[] values)
    {
        return Array.IndexOf(values, @this) != -1;
    }

    public static IEnumerable<int> IndicesOf(this string text, string searchFor)
    {
        if (string.IsNullOrEmpty(searchFor)) yield break;

        var lastLoc = text.IndexOf(searchFor, StringComparison.Ordinal);
        while (lastLoc != -1)
        {
            yield return lastLoc;
            lastLoc = text.IndexOf(searchFor, lastLoc + 1, StringComparison.Ordinal);
        }
    }

    public static bool IsAlphabetic(this string @this)
    {
        return !Regex.IsMatch(@this, "[^a-zA-Z]");
    }

    public static bool IsAlphabeticNumeric(this string @this)
    {
        return !Regex.IsMatch(@this, "[^a-zA-Z0-9]");
    }

    public static bool IsAlphanumeric(this string s)
    {
        return s.ToCharArray().All(char.IsLetterOrDigit);
    }

    public static bool IsBase64(this string base64)
    {
        try
        {
            var _ = Convert.FromBase64String(base64);
            return true;
        }
        catch (Exception)
        {
            // ignored
        }
        return false;
    }

    public static bool IsControl(this string s, int index)
    {
        return char.IsControl(s, index);
    }

    public static bool IsDateTime(this string date)
    {
        var isDate = true;
        try
        {
            // ReSharper disable once UnusedVariable
            var dt = DateTime.Parse(date);
        }
        catch
        {
            isDate = false;
        }

        return isDate;
    }

    public static bool IsDigit(this string str)
    {
        return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
    }

    public static bool IsDigit(this string s, int index)
    {
        return char.IsDigit(s, index);
    }

    public static bool IsEmail(this string @this)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        var match = Regex.Match(@this, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        return match.Success;
    }

    public static bool IsEmpty(this string @this)
    {
        return @this == "";
    }

    public static bool IsHighSurrogate(this string s, int index)
    {
        return char.IsHighSurrogate(s, index);
    }

    public static string? IsInterned(this string str)
    {
        return string.IsInterned(str);
    }

    public static bool IsIpAddress(this string ip, out IPAddress? ipAddress)
    {
        if (IPAddress.TryParse(ip, out var address))
        {
            switch (address.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                case AddressFamily.InterNetworkV6:
                    ipAddress = address;
                    return true;
            }
        }
        ipAddress = null;
        return false;
    }
    public static bool IsIpAddress(this string ipAddress)
    {
        return ipAddress.IsIpAddressV4() || ipAddress.IsIpAddressV6();
    }

    public static bool IsIpAddressV4(this string ipAddress)
    {
        if (!IPAddress.TryParse(ipAddress, out var address)) return false;

        return address.AddressFamily switch
        {
            AddressFamily.InterNetwork => true,
            _ => false
        };
    }

    public static bool IsIpAddressV6(this string ipAddress)
    {
        if (!IPAddress.TryParse(ipAddress, out var address)) return false;

        return address.AddressFamily switch
        {
            AddressFamily.InterNetworkV6 => true,
            _ => false
        };
    }

    public static bool IsLetter(this string s, int index)
    {
        return char.IsLetter(s, index);
    }

    public static bool IsLetterOrDigit(this string s, int index)
    {
        return char.IsLetterOrDigit(s, index);
    }

    public static bool IsLike(this string @this, string pattern)
    {
        // Turn the pattern into regex pattern, and match the whole string with ^$
        var regexPattern = "^" + Regex.Escape(pattern) + "$";

        // Escape special character ?, #, *, [], and [!]
        regexPattern = regexPattern.Replace(@"\[!", "[^")
            .Replace(@"\[", "[")
            .Replace(@"\]", "]")
            .Replace(@"\?", ".")
            .Replace(@"\*", ".*")
            .Replace(@"\#", @"\d");

        return Regex.IsMatch(@this, regexPattern);
    }

    public static bool IsLower(this string s, int index)
    {
        return char.IsLower(s, index);
    }

    public static bool IsLowSurrogate(this string s, int index)
    {
        return char.IsLowSurrogate(s, index);
    }

    public static bool IsMatch(this string input, string pattern)
    {
        return Regex.IsMatch(input, pattern);
    }

    public static bool IsMatch(this string input, string pattern, RegexOptions options)
    {
        return Regex.IsMatch(input, pattern, options);
    }

    public static bool IsMatchingTo(this string value, string regexPattern)
    {
        return IsMatchingTo(value, regexPattern, RegexOptions.None);
    }

    public static bool IsMatchingTo(this string value, string regexPattern, RegexOptions options)
    {
        return Regex.IsMatch(value, regexPattern, options);
    }

    public static bool IsMultiLine(this string text, int startPosition, int endPosition)
    {
        var start = GetPosition(text, startPosition);
        var end = GetPosition(text, endPosition);
        return start.Line != end.Line;
    }

    public static bool IsNotEmpty(this string? @this)
    {
        return @this != "";
    }

    public static bool IsNotNull(this string? @this)
    {
        return @this != null;
    }

    public static bool IsNotNullOrEmpty(this string @this)
    {
        return !string.IsNullOrEmpty(@this);
    }

    public static bool IsNotNullOrEmptyOrWhiteSpace(this string value)
    {
        return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNotNullOrWhiteSpace(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNull(this string? @this)
    {
        return @this == null;
    }

    public static bool IsNullOrEmpty(this string @this)
    {
        return string.IsNullOrEmpty(@this);
    }

    public static bool IsNullOrEmptyOrWhiteSpace(this string value)
    {
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNullOrWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNumber(this string s, int index)
    {
        return char.IsNumber(s, index);
    }

    public static bool IsNumeric(this string @this)
    {
        return !Regex.IsMatch(@this, "[^0-9]");
    }

    public static bool IsPunctuation(this string s, int index)
    {
        return char.IsPunctuation(s, index);
    }

    public static bool IsRegexMatch(this string input, string regex)
    {
        return Regex.Match(input, regex, RegexOptions.Compiled).Success;
    }

    public static bool IsRegexMatch(this string input, Regex regex)
    {
        return regex.Match(input).Success;
    }

    public static bool IsSeparator(this string s, int index)
    {
        return char.IsSeparator(s, index);
    }

    public static bool IsSurrogate(this string s, int index)
    {
        return char.IsSurrogate(s, index);
    }

    public static bool IsSurrogatePair(this string s, int index)
    {
        return char.IsSurrogatePair(s, index);
    }

    public static bool IsSymbol(this string s, int index)
    {
        return char.IsSymbol(s, index);
    }

    public static bool IsUpper(this string s, int index)
    {
        return char.IsUpper(s, index);
    }

    public static bool IsUri(this string @this)
    {
        try
        {
            var _ = new Uri(@this);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsUrl(this string @this)
    {
        try
        {
            var _ = new Uri(@this);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidEmail(this string email)
    {
        var trimmedEmail = email.Trim();
        if (trimmedEmail.EndsWith("."))
        {
            return false;
        }
        try
        {
            var address = new System.Net.Mail.MailAddress(email);
            return address.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
    public static bool IsValidNumber(this string number)
    {
        return IsValidNumber(number, CultureInfo.CurrentCulture);
    }

    public static bool IsValidNumber(this string number, CultureInfo culture)
    {
        string _validNumberPattern =
            @"^-?(?:\d+|\d{1,3}(?:"
            + culture.NumberFormat.NumberGroupSeparator +
            @"\d{3})+)?(?:\"
            + culture.NumberFormat.NumberDecimalSeparator +
            @"\d+)?$";

        return new Regex(_validNumberPattern, RegexOptions.ECMAScript).IsMatch(number);
    }

    public static bool IsValidRegex(this string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) return false;
        try
        {
            var _ = Regex.Match("", pattern);
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }

    public static bool IsValidUri(this string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    public static bool IsValidUrl(this string text)
    {
        var rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        return rx.IsMatch(text);
    }

    public static bool IsWhiteSpace(this string s, int index)
    {
        return char.IsWhiteSpace(s, index);
    }

    public static string JavaScriptStringEncode(this string value)
    {
        return HttpUtility.JavaScriptStringEncode(value);
    }

    public static string JavaScriptStringEncode(this string value, bool addDoubleQuotes)
    {
        return HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes);
    }

    public static string Join(this string separator, string[] value)
    {
        return string.Join(separator, value);
    }

    public static string Join(this string separator, object[] values)
    {
        return string.Join(separator, values);
    }

    public static string Join<T>(this string separator, IEnumerable<T> values)
    {
        return string.Join(separator, values);
    }

    public static string Join(this string separator, IEnumerable<string> values)
    {
        return string.Join(separator, values);
    }

    public static string Join(this string separator, string[] value, int startIndex, int count)
    {
        return string.Join(separator, value, startIndex, count);
    }

    public static string? LastChar(this string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            if (input.Length >= 1)
                return input[^1].ToString();
            return input;
        }

        return null;
    }

    public static string Left(this string @this, int length)
    {
        return @this.Substring(0, length);
    }

    public static string LeftSafe(this string @this, int length)
    {
        return @this.Substring(0, Math.Min(length, @this.Length));
    }

    public static bool Like(this string value, string search)
    {
        return value.Contains(search) || value.StartsWith(search) || value.EndsWith(search);
    }

    public static Match Match(this string input, string pattern)
    {
        return Regex.Match(input, pattern);
    }

    public static Match Match(this string input, string pattern, RegexOptions options)
    {
        return Regex.Match(input, pattern, options);
    }

    public static MatchCollection Matches(this string input, string pattern)
    {
        return Regex.Matches(input, pattern);
    }

    public static MatchCollection Matches(this string input, string pattern, RegexOptions options)
    {
        return Regex.Matches(input, pattern, options);
    }

    public static bool MatchWords(this string text, string word)
    {
        var tokens = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToLower());
        return tokens.Any(t => word.Trim().ToLower().Contains(t.Trim()));
    }

    public static bool NotIn(this string @this, params string[] values)
    {
        return Array.IndexOf(values, @this) == -1;
    }

    public static int NthIndexOf(this string input, string match, int occurrence)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        if (match == null) throw new ArgumentNullException(nameof(match));

        const int notFoundValue = -1;

        if (occurrence < 1) throw new ArgumentException("occurrence equal to 1 or larger.", nameof(occurrence));

        var i = 1;
        var index = 0;

        while (i <= occurrence &&
               (index = input.IndexOf(match, index + 1, StringComparison.Ordinal)) != notFoundValue)
        {
            if (i == occurrence)
                return index;

            i++;
        }

        return notFoundValue;
    }

    public static string? NullIfEmpty(this string @this)
    {
        return @this == "" ? null : @this;
    }

    public static string? NullIfWhiteSpace(this string @this)
    {
        return !@this.IsNullOrWhiteSpace() ? @this : null;
    }

    public static string PadBoth(this string value, int width, char padChar, bool truncate = false)
    {
        var diff = width - value.Length;
        if (diff == 0 || (diff < 0 && !truncate)) return value;
        if (diff < 0) return value.Substring(0, width);
        return value.PadLeft(width - diff / 2, padChar).PadRight(width, padChar);
    }

    public static Uri ParseAsUri(this string uri)
    {
        try
        {
            return new Uri(uri);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public static DateTime ParseAsUtc(this string str)
    {
        return DateTimeOffset.Parse(str).UtcDateTime;
    }

    public static T ParseEnum<T>(this string name, bool ignoreCase = false)
        where T : struct, Enum
    {
        return (T)Enum.Parse(typeof(T), name, ignoreCase);
    }

    public static NameValueCollection ParseQueryString(this string query)
    {
        return HttpUtility.ParseQueryString(query);
    }

    public static NameValueCollection ParseQueryString(this string query, Encoding encoding)
    {
        return HttpUtility.ParseQueryString(query, encoding);
    }

    public static string PathCombine(this string @this, params string[] paths)
    {
        var list = paths.ToList();
        list.Insert(0, @this);
        return Path.Combine(list.ToArray());
    }

    public static string PathRelativeTo(this string path, string root)
    {
        var pathParts = path.GetPathParts().ToList();
        var rootParts = root.GetPathParts().ToList();

        var length = pathParts.Count > rootParts.Count ? rootParts.Count : pathParts.Count;
        for (int i = 0; i < length; i++)
        {
            if (pathParts.First() == rootParts.First())
            {
                pathParts.RemoveAt(0);
                rootParts.RemoveAt(0);
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < rootParts.Count; i++)
        {
            pathParts.Insert(0, "..");
        }

        return pathParts.Count > 0 ? Path.Combine(pathParts.ToArray()) : string.Empty;
    }

    public static string Pluralize(this string word)
    {
        return PluralizeHelper.Pluralize(word);
    }

    public static void ReadLines(this string text, Action<string> callback)
    {
        var reader = new StringReader(text);
        while (reader.ReadLine() is { } line)
        {
            callback(line);
        }
    }

    public static IEnumerable<string> ReadLines(this string text)
    {
        var reader = new StringReader(text);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    public static Match RegexMatch(this string @this, string pattern, RegexOptions regexOption)
    {
        return Regex.Match(@this, pattern, regexOption);
    }

    public static Match RegexMatch(this string @this, string pattern)
    {
        return @this.RegexMatch(pattern, RegexOptions.None);
    }

    public static MatchCollection RegexMatches(this string @this, string pattern, RegexOptions regexOption)
    {
        return Regex.Matches(@this, pattern, regexOption);
    }

    public static MatchCollection RegexMatches(this string @this, string pattern)
    {
        return @this.RegexMatches(pattern, RegexOptions.None);
    }

    public static string RemoveAccents(string input)
    {
        return new string(input
            .Normalize(NormalizationForm.FormD)
            .ToCharArray()
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());
    }

    public static string RemoveAll(this string source, params string[] removeStrings)
    {
        var v = source;
        foreach (var s in removeStrings) v = v.Replace(s, string.Empty);
        return v;
    }

    public static string RemoveAllSpecialCharacters(this string value)
    {
        var sb = new StringBuilder(value.Length);
        foreach (var c in value.Where(char.IsLetterOrDigit))
            sb.Append(c);
        return sb.ToString();
    }

    public static string RemoveBefore(this string @this, char c, bool removeChar = true)
    {
        return @this.Substring(removeChar ? @this.IndexOf(c) + 1 : @this.IndexOf(c));
    }

    public static string RemoveBeforeLastIndex(this string @this, char c, bool removeChar = true)
    {
        return @this.Substring(removeChar ? @this.LastIndexOf(c) + 1 : @this.LastIndexOf(c));
    }

    public static string RemoveControlCharacters(this string input)
    {
        return
            input.Where(character => !char.IsControl(character))
                .Aggregate(new StringBuilder(), (builder, character) => builder.Append(character))
                .ToString();
    }

    public static string RemoveDiacritics(this string value)
    {
        var normalizedString = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
            else if (c == 1619) //آ
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string RemoveEmptyLines(this string text)
    {
        return Regex.Replace(text, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
    }

    public static string RemoveEnd(this string original, string suffix)
    {
        if (!original.EndsWith(suffix))
            return original;
        return original.Substring(0, original.Length - suffix.Length);
    }

    public static string RemoveEnd(this string original, string suffix, StringComparison comparison)
    {
        if (!original.EndsWith(suffix, comparison))
            return original;
        return original.Substring(0, original.Length - suffix.Length);
    }

    public static string RemoveFirst(this string instr, int number)
    {
        return instr.Substring(number);
    }

    public static string RemoveFirstAndLast(this string str)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentException("Value cannot be null or empty.", nameof(str));
        if (str.Length < 2)
        {
            throw new Exception($"Length of {nameof(str)} cannot be less than 2.");
        }

        return str.Substring(1, str.Length - 2);
    }

    public static string RemoveFirstAndLastChars(this string str)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentException("Value cannot be null or empty.", nameof(str));
        if (str.Length < 2)
        {
            throw new Exception($"Length of {nameof(str)} cannot be less than 2.");
        }

        return str.Substring(1, str.Length - 2);
    }

    public static string RemoveFirstCharacter(this string instr)
    {
        return instr.Substring(1);
    }

    public static string RemoveHtmlTags(this string htmlString)
    {
        return Regex.Replace(htmlString, @"<[^>]*(>|$)|&nbsp;|&zwnj;|&raquo;|&laquo;", string.Empty).Trim();
    }

    public static string RemoveLast(this string instr, int number)
    {
        return instr.Substring(0, instr.Length - number);
    }

    public static string RemoveLastCharacter(this string instr)
    {
        return instr.Substring(0, instr.Length - 1);
    }

    public static string RemoveLetter(this string @this)
    {
        return new string(@this.ToCharArray().Where(x => !char.IsLetter(x)).ToArray());
    }

    public static string RemoveNumber(this string @this)
    {
        return new string(@this.ToCharArray().Where(x => !char.IsNumber(x)).ToArray());
    }

    public static string RemovePrefix(this string @this, string prefix,
        StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        if (prefix == null) throw new ArgumentNullException(nameof(prefix));
        return @this.StartsWith(prefix, stringComparison)
            ? @this.Substring(prefix.Length, @this.Length - prefix.Length)
            : @this;
    }

    public static string RemoveStart(this string original, string prefix)
    {
        if (!original.StartsWith(prefix))
            return original;
        return original.Substring(prefix.Length);
    }

    public static string RemoveStart(this string original, string prefix, StringComparison comparison)
    {
        if (!original.StartsWith(prefix, comparison))
            return original;
        return original.Substring(prefix.Length);
    }

    public static string RemoveSuffix(this string @this, string suffix,
        StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        if (suffix == null) throw new ArgumentNullException(nameof(suffix));
        return @this.EndsWith(suffix, stringComparison)
            ? @this.Substring(0, @this.Length - suffix.Length)
            : @this;
    }

    public static string RemoveWhere(this string @this, Func<char, bool> predicate)
    {
        return new string(@this.ToCharArray().Where(x => !predicate(x)).ToArray());
    }

    public static string RemoveWhiteSpaces(this string text)
    {
        return string.IsNullOrEmpty(text) ? text : Regex.Replace(text, @"\s+", string.Empty).Trim();
    }

    public static string Repeat(this string @this, int repeatCount)
    {
        if (@this.Length == 1) return new string(@this[0], repeatCount);

        var sb = new StringBuilder(repeatCount * @this.Length);
        while (repeatCount-- > 0) sb.Append(@this);

        return sb.ToString();
    }

    public static string Replace(this string text, int startIndex, int length, string replacement)
    {
        return text.Remove(startIndex, length).Insert(startIndex, replacement);
    }

    public static string Replace(this string @this, string regexPattren, string replacement)
    {
        return Regex.Replace(@this, regexPattren, replacement, RegexOptions.Compiled);
    }

    public static string Replace(this string @this, string regexPattren, string replacement, RegexOptions regexOptions)
    {
        return Regex.Replace(@this, regexPattren, replacement, regexOptions);
    }

    public static string Replace(this string @this, string pattern, MatchEvaluator evaluator)
    {
        return Regex.Replace(@this, pattern, evaluator);
    }

    public static string ReplaceAll(this string value, IEnumerable<string> oldValues,
        Func<string, string> replacePredicate)
    {
        var sbStr = new StringBuilder(value);
        foreach (var oldValue in oldValues)
        {
            var newValue = replacePredicate(oldValue);
            sbStr.Replace(oldValue, newValue);
        }

        return sbStr.ToString();
    }

    public static string ReplaceAll(this string value, IEnumerable<string> oldValues, string newValue)
    {
        var sbStr = new StringBuilder(value);
        foreach (var oldValue in oldValues)
            sbStr.Replace(oldValue, newValue);

        return sbStr.ToString();
    }

    public static string ReplaceAll(this string value, IEnumerable<string> oldValues, IEnumerable<string> newValues)
    {
        var sbStr = new StringBuilder(value);
        var newValueEnum = newValues.GetEnumerator();
        foreach (var old in oldValues)
        {
            if (!newValueEnum.MoveNext())
                throw new ArgumentOutOfRangeException(nameof(newValues),
                    "newValues sequence is shorter than oldValues sequence");
            sbStr.Replace(old, newValueEnum.Current ?? throw new InvalidOperationException());
        }

        if (newValueEnum.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(newValues),
                "newValues sequence is longer than oldValues sequence");
        newValueEnum.Dispose();
        return sbStr.ToString();
    }

    public static string ReplaceAt(this string str, int index, string replace)
    {
        var length = replace.Length;
        return str.Remove(index, Math.Min(length, str.Length - index))
            .Insert(index, replace);
    }

    public static string ReplaceAt(this string str, int index, int length, string replace)
    {
        return str.Remove(index, Math.Min(length, str.Length - index))
            .Insert(index, replace);
    }

    public static string ReplaceAt(this string str, int startIndex, long endIndex, string replace)
    {
        var length = endIndex - startIndex;
        return str.Remove(startIndex, Math.Min(Convert.ToInt32(length), str.Length - startIndex))
            .Insert(startIndex, replace);
    }

    public static string ReplaceAt(this string text, char target, string replace,
        ReplaceMode replaceMode = ReplaceMode.On)
    {
        var i = text.IndexOf(target);
        if (i > -1)
        {
            var part1 = text.Substring(0, i);
            var part2 = text.Substring(i + 1);
            switch (replaceMode)
            {
                case ReplaceMode.On:
                    return part1 + replace + part2;

                case ReplaceMode.Before:
                    return part1 + replace + target + part2;

                case ReplaceMode.After:
                    return part1 + target + replace + part2;
            }

            return text;
        }

        return text;
    }

    public static string ReplaceByEmpty(this string @this, params string[] values)
    {
        foreach (var value in values) @this = @this.Replace(value, "");

        return @this;
    }

    public static string ReplaceExceptLastOccurrence(this string input, char pattern, char replacement)
    {
        return Regex.Replace(input, $"[{pattern}](?=.*[{pattern}])", replacement.ToString());
    }

    public static string ReplaceFirst(this string @this, string oldValue, string newValue)
    {
        var startIndex = @this.IndexOf(oldValue, StringComparison.Ordinal);
        return startIndex == -1 ? @this : @this.Remove(startIndex, oldValue.Length).Insert(startIndex, newValue);
    }

    public static string ReplaceFirst(this string @this, int number, string oldValue, string newValue)
    {
        var list = @this.Split(oldValue).ToList();
        var old = number + 1;
        var listStart = list.Take(old);
        var listEnd = list.Skip(old).ToList();

        return string.Join(newValue, listStart) +
               (listEnd.Any() ? oldValue : "") +
               string.Join(oldValue, listEnd);
    }

    public static string ReplaceIgnoreCase(this string input, string pattern, string replacement)
    {
        return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
    }

    public static string ReplaceLast(this string @this, string oldValue, string newValue)
    {
        var startIndex = @this.LastIndexOf(oldValue, StringComparison.Ordinal);
        return startIndex == -1 ? @this : @this.Remove(startIndex, oldValue.Length).Insert(startIndex, newValue);
    }

    public static string ReplaceLast(this string @this, int number, string oldValue, string newValue)
    {
        var list = @this.Split(oldValue).ToList();
        var old = Math.Max(0, list.Count - number - 1);
        var listStart = list.Take(old);
        var listEnd = list.Skip(old);

        return string.Join(oldValue, listStart) +
               (old > 0 ? oldValue : "") +
               string.Join(newValue, listEnd);
    }

    public static string ReplaceRegex(this string value, string regexPattern, string replaceValue,
        RegexOptions options)
    {
        return Regex.Replace(value, regexPattern, replaceValue, options);
    }

    public static string ReplaceWhenEquals(this string @this, string oldValue, string newValue)
    {
        return @this == oldValue ? newValue : @this;
    }

    public static string ReplaceWhiteSpacesWithOne(this string input)
    {
        return Regex.Replace(input, @"\s+", " ");
    }

    public static string ReplaceWith(this string value, string regexPattern, string replaceValue)
    {
        return ReplaceWith(value, regexPattern, replaceValue, RegexOptions.None);
    }

    public static string ReplaceWith(this string value, string regexPattern, string replaceValue,
        RegexOptions options)
    {
        return Regex.Replace(value, regexPattern, replaceValue, options);
    }

    public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator)
    {
        return ReplaceWith(value, regexPattern, RegexOptions.None, evaluator);
    }

    public static string ReplaceWith(this string value, string regexPattern, RegexOptions options,
        MatchEvaluator evaluator)
    {
        return Regex.Replace(value, regexPattern, evaluator, options);
    }

    public static string Reverse(this string @this)
    {
        if (@this.Length <= 1) return @this;

        var chars = @this.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static string Right(this string @this, int length)
    {
        return @this.Substring(@this.Length - length);
    }

    public static string RightSafe(this string @this, int length)
    {
        return @this.Substring(Math.Max(0, @this.Length - length));
    }

    public static void SaveAs(this string @this, string fileName, bool append = false)
    {
        using TextWriter tw = new StreamWriter(fileName, append);
        tw.Write(@this);
    }

    public static void SaveAs(this string @this, FileInfo file, bool append = false)
    {
        using TextWriter tw = new StreamWriter(file.FullName, append);
        tw.Write(@this);
    }

    public static string Singularize(this string word)
    {
        return PluralizeHelper.Singularize(word);
    }

    public static string Slice(this string source, int start, int end)
    {
        if (end < 0) // Keep this for negative end support
            end = source.Length + end;
        var len = end - start; // Calculate length
        return source.Substring(start, len); // Return Substring of length
    }

    public static string[] Split(this string @this, string separator, StringSplitOptions option)
    {
        return @this.Split(new[] { separator }, option);
    }

    public static string[] Split(this string str, int chunkSize)
    {
        return
            Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize)).ToArray();
    }

    public static string[] Split(this string value, string regexPattern)
    {
        return value.Split(regexPattern, RegexOptions.None);
    }

    public static string[] Split(this string value, string regexPattern, RegexOptions options)
    {
        return Regex.Split(value, regexPattern, options);
    }

    public static IEnumerable<string> SplitAndKeepSeparators(this string text, params char[] separator)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));
        if (separator == null) throw new ArgumentNullException(nameof(separator));

        var start = 0;
        int index;
        while ((index = text.IndexOfAny(separator, start)) != -1)
        {
            if (index - start > 0)
                yield return text.Substring(start, index - start);
            yield return text.Substring(index, 1);
            start = index + 1;
        }

        if (start < text.Length)
        {
            yield return text.Substring(start);
        }
    }

    public static string SplitCamelCase(this string str)
    {
        return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }

    public static string SplitPascalCase(this string str)
    {
        return SplitCamelCase(str);
    }

    public static SqlDbType SqlTypeNameToSqlDbType(this string @this)
    {
        switch (@this.ToLower())
        {
            case "image": // 34 | "image" | SqlDbType.Image
                return SqlDbType.Image;

            case "text": // 35 | "text" | SqlDbType.Text
                return SqlDbType.Text;

            case "uniqueidentifier": // 36 | "uniqueidentifier" | SqlDbType.UniqueIdentifier
                return SqlDbType.UniqueIdentifier;

            case "date": // 40 | "date" | SqlDbType.Date
                return SqlDbType.Date;

            case "time": // 41 | "time" | SqlDbType.Time
                return SqlDbType.Time;

            case "datetime2": // 42 | "datetime2" | SqlDbType.DateTime2
                return SqlDbType.DateTime2;

            case "datetimeoffset": // 43 | "datetimeoffset" | SqlDbType.DateTimeOffset
                return SqlDbType.DateTimeOffset;

            case "tinyint": // 48 | "tinyint" | SqlDbType.TinyInt
                return SqlDbType.TinyInt;

            case "smallint": // 52 | "smallint" | SqlDbType.SmallInt
                return SqlDbType.SmallInt;

            case "int": // 56 | "int" | SqlDbType.Int
                return SqlDbType.Int;

            case "smalldatetime": // 58 | "smalldatetime" | SqlDbType.SmallDateTime
                return SqlDbType.SmallDateTime;

            case "real": // 59 | "real" | SqlDbType.Real
                return SqlDbType.Real;

            case "money": // 60 | "money" | SqlDbType.Money
                return SqlDbType.Money;

            case "datetime": // 61 | "datetime" | SqlDbType.DateTime
                return SqlDbType.DateTime;

            case "float": // 62 | "float" | SqlDbType.Float
                return SqlDbType.Float;

            case "sql_variant": // 98 | "sql_variant" | SqlDbType.Variant
                return SqlDbType.Variant;

            case "ntext": // 99 | "ntext" | SqlDbType.NText
                return SqlDbType.NText;

            case "bit": // 104 | "bit" | SqlDbType.Bit
                return SqlDbType.Bit;

            case "decimal": // 106 | "decimal" | SqlDbType.Decimal
                return SqlDbType.Decimal;

            case "numeric": // 108 | "numeric" | SqlDbType.Decimal
                return SqlDbType.Decimal;

            case "smallmoney": // 122 | "smallmoney" | SqlDbType.SmallMoney
                return SqlDbType.SmallMoney;

            case "bigint": // 127 | "bigint" | SqlDbType.BigInt
                return SqlDbType.BigInt;

            case "varbinary": // 165 | "varbinary" | SqlDbType.VarBinary
                return SqlDbType.VarBinary;

            case "varchar": // 167 | "varchar" | SqlDbType.VarChar
                return SqlDbType.VarChar;

            case "binary": // 173 | "binary" | SqlDbType.Binary
                return SqlDbType.Binary;

            case "char": // 175 | "char" | SqlDbType.Char
                return SqlDbType.Char;

            case "timestamp": // 189 | "timestamp" | SqlDbType.Timestamp
                return SqlDbType.Timestamp;

            case "nvarchar": // 231 | "nvarchar", "sysname" | SqlDbType.NVarChar
                return SqlDbType.NVarChar;

            case "sysname": // 231 | "nvarchar", "sysname" | SqlDbType.NVarChar
                return SqlDbType.NVarChar;

            case "nchar": // 239 | "nchar" | SqlDbType.NChar
                return SqlDbType.NChar;

            case "hierarchyid": // 240 | "hierarchyid", "geometry", "geography" | SqlDbType.Udt
                return SqlDbType.Udt;

            case "geometry": // 240 | "hierarchyid", "geometry", "geography" | SqlDbType.Udt
                return SqlDbType.Udt;

            case "geography": // 240 | "hierarchyid", "geometry", "geography" | SqlDbType.Udt
                return SqlDbType.Udt;

            case "xml": // 241 | "xml" | SqlDbType.Xml
                return SqlDbType.Xml;

            default:
                throw new Exception(
                    $"Unsupported Type: {@this}. Please let us know about this type and we will support it: sales@zzzprojects.com");
        }
    }

    public static bool StartsWithIgnoreCase(this string a, string b)
    {
        return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
    }

    public static string Strip(this string s, char character)
    {
        s = s.Replace(character.ToString(), "");
        return s;
    }

    public static string Strip(this string s, params char[] chars)
    {
        foreach (var c in chars)
            s = s.Replace(c.ToString(), "");
        return s;
    }

    public static string Strip(this string s, string subString)
    {
        s = s.Replace(subString, "");
        return s;
    }

    public static string SubstringAfter(this string s, string sub, StringComparison comparison)
    {
        var index = s.IndexOf(sub, comparison);
        if (index < 0)
            return string.Empty;
        return s.Substring(index + sub.Length, s.Length - index - sub.Length);
    }

    public static string SubstringAfter(this string original, string value)
    {
        // ReSharper disable once StringIndexOfIsCultureSpecific.1
        return original.SubstringAfter(original.IndexOf(value), value.Length);
    }

    public static string SubstringAfterLast(this string s, string sub, StringComparison comparsion)
    {
        var index = s.LastIndexOf(sub, comparsion);
        if (index < 0)
            return string.Empty;
        return s.Substring(index + sub.Length, s.Length - index - sub.Length);
    }

    public static string SubstringAfterLast(this string original, string value)
    {
        // ReSharper disable once StringLastIndexOfIsCultureSpecific.1
        return original.SubstringAfter(original.LastIndexOf(value), value.Length);
    }

    public static string SubstringBefore(this string original, string value)
    {
        // ReSharper disable once StringIndexOfIsCultureSpecific.1
        return original.SubstringBefore(original.IndexOf(value));
    }

    public static string SubstringBefore(this string original, string value,
        StringComparison comparisonType)
    {
        return original.SubstringBefore(original.IndexOf(value, comparisonType));
    }

    public static string SubstringBeforeLast(this string original, string value)
    {
        // ReSharper disable once StringLastIndexOfIsCultureSpecific.1
        return original.SubstringBefore(original.LastIndexOf(value));
    }

    public static string SubstringBeforeLast(this string original, string value, StringComparison comparisonType)
    {
        return original.SubstringBefore(original.LastIndexOf(value, comparisonType));
    }

    public static string SubstringByIndex(this string source, int startIndex, int endIndex)
    {
        return source.Substring(startIndex, endIndex - startIndex);
    }

    public static string SubstringTillEnd(this string source, int n)
    {
        if (n >= source.Length) return source;
        return source.Substring(source.Length - n);
    }

    public static string SubstringUntil(this string s, string sub,
        StringComparison comparison = StringComparison.Ordinal)
    {
        var index = s.IndexOf(sub, comparison);
        if (index < 0)
            return s;
        return s.Substring(0, index);
    }

    public static string SubstringUntilLast(this string s, string sub,
        StringComparison comparison = StringComparison.Ordinal)
    {
        var index = s.LastIndexOf(sub, comparison);
        if (index < 0)
            return s;
        return s.Substring(0, index);
    }

    public static string TabOrWhiteSpaceHandler(this string text,
        StringSplitOptions stringSplitOptions = StringSplitOptions.None)
    {
        // \$tb3 or \$ws13
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        var sb = new StringBuilder();
        var lines = text.Trim().Split(new[] { "\r\n", "\r", "\n" }, stringSplitOptions);
        foreach (var line in lines)
        {
            var txt = line.Trim();
            var matches = TabOrWhiteSpaceRegex.Matches(txt);
            foreach (Match match in matches)
            {
                var gr2 = match.Groups[2].Value;
                var gr4 = match.Groups[4].Value;
                if (!string.IsNullOrEmpty(gr2)) txt = txt.Replace(match.Value, new string('\t', Convert.ToInt32(gr2)));

                if (!string.IsNullOrEmpty(gr4)) txt = txt.Replace(match.Value, new string(' ', Convert.ToInt32(gr4)));
            }

            sb.AppendLine(txt);
        }

        return sb.ToString();
    }

    public static string ToAlphaNumericOnly(this string text, string replacement = "")
    {
        var regex = new Regex("[^a-zA-Z0-9]");
        return regex.Replace(text, replacement);
    }

    public static string ToAlphaNumericUnderlineOnly(this string text, string replacement = "")
    {
        var regex = new Regex("[^a-zA-Z0-9_]");
        return regex.Replace(text, replacement);
    }

    public static byte[] ToAsciiByteArray(this string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }

    public static string ToBase62String(this string text, Base62CharacterSet charSet = Base62CharacterSet.Default)
    {
        var base62 = new Base62Converter(charSet);
        return base62.Encode(text);
    }

    public static string ToBase64String(this string value)
    {
        return value.ToBase64String(Encoding.UTF8);
    }

    public static string ToBase64String(this string value, Encoding encoding)
    {
        var bytes = encoding.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    public static bool ToBool(this string stringValue)
    {
        if (string.IsNullOrEmpty(stringValue)) return false;

        return bool.Parse(stringValue);
    }

    public static bool ToBoolean(this string value, bool defaultValue)
    {
        if (bool.TryParse(value, out var result)) return result;
        return defaultValue;
    }

    public static byte[] ToByteArray(this string @this)
    {
        Encoding encoding = Activator.CreateInstance<UTF8Encoding>();
        return encoding.GetBytes(@this);
    }

    public static byte[] ToByteArray<TEncoding>(this string str) where TEncoding : Encoding
    {
        Encoding enc = Activator.CreateInstance<TEncoding>();
        return enc.GetBytes(str);
    }

    public static byte[] ToBytes(this string @this, Encoding encoding)
    {
        if (encoding == null) throw new ArgumentNullException(nameof(encoding), "encoding is null");
        return encoding.GetBytes(@this);
    }

    public static byte[] ToBytes(this string @this)
    {
        return @this.ToBytes(Encoding.UTF8);
    }

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        var output = str.ToTitleCase();
        return char.ToLower(output[0]) + output.Substring(1);
    }

    public static IEnumerable<char> ToChars(this string str)
    {
        return str.Select(x => x);
    }

    public static string[] ToDelimitedArray(this string content, char delimiter = ',')
    {
        string[] array = content.Split(delimiter);
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = array[i].Trim();
        }

        return array;
    }

    public static DirectoryInfo ToDirectoryInfo(this string @this)
    {
        return new DirectoryInfo(@this);
    }

    public static T ToEnum<T>(this string @this) where T : Enum
    {
        var enumType = typeof(T);
        return (T)Enum.Parse(enumType, @this);
    }

    public static void ToFile(this string fileText, FileInfo fileInfo)
    {
        if (fileInfo == null)
            throw new ArgumentNullException(nameof(fileInfo));

        if (fileInfo.Directory is { Exists: false }) fileInfo.Directory.Create();
        using var writer = new StreamWriter(fileInfo.FullName, true);
        writer.Write(fileText);
    }

    public static FileInfo ToFileInfo(this string @this)
    {
        return new FileInfo(@this);
    }

    public static IEnumerable<string> ToLines(this string str,
        StringSplitOptions stringSplitOptions = StringSplitOptions.RemoveEmptyEntries)
    {
        return str.Split(new[] { Environment.NewLine }, stringSplitOptions);
    }

    public static MemoryStream ToMemoryStream(this string? text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text ?? ""));
    }

    public static char? ToNullableChar(this string input)
    {
        if (string.IsNullOrEmpty(input) || input.Trim().Length == 0)
            return new char?();
        if (input.Trim().Length > 1)
            throw new ArgumentException($"Cannot convert string({input.Trim().Length}) to char?");
        return input[0];
    }

    public static string ToPersianNumbers(this string data, bool replaceDotWithComma = false)
    {
        if (string.IsNullOrEmpty(data)) return string.Empty;
        data = data
            .Replace("0", "\u06F0")
            .Replace("1", "\u06F1")
            .Replace("2", "\u06F2")
            .Replace("3", "\u06F3")
            .Replace("4", "\u06F4")
            .Replace("5", "\u06F5")
            .Replace("6", "\u06F6")
            .Replace("7", "\u06F7")
            .Replace("8", "\u06F8")
            .Replace("9", "\u06F9");

        return replaceDotWithComma ? data.Replace(".", ",") : data;
    }

    public static SecureString ToSecureString(this string @this)
    {
        var secureString = new SecureString();
        foreach (var c in @this)
            secureString.AppendChar(c);

        return secureString;
    }

    public static Stream ToStream(this string? value)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
    }

    public static Stream ToStream(this string? value, Encoding? encoding)
    {
        return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(value ?? string.Empty));
    }

    public static string ToTitleCase(this string @this, CultureInfo culture)
    {
        return culture.TextInfo.ToTitleCase(@this);
    }

    public static string ToTitleCase(this string @this)
    {
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(@this);
    }

    public static string ToUnderscoreCase(this string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }

    public static string ToUnicodeString(this string text, bool ignoreWhiteSpaces = false)
    {
        var builder = new StringBuilder();
        foreach (var ch in text)
            if (ignoreWhiteSpaces && char.IsWhiteSpace(ch))
            {
                builder.Append(" ");
            }
            else
            {
                builder.Append(@"\u");
                builder.AppendFormat("{0:x4}", (int)ch);
            }

        return builder.ToString();
    }

    public static byte[] ToUtf8ByteArray(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static DateTime? ToValidDateTimeOrNull(this string @this)
    {
        if (DateTime.TryParse(@this, out var date)) return date;
        return null;
    }

    public static IEnumerable<string> ToWords(this string str, string[]? wordSeparators = null)
    {
        var status = wordSeparators == null || wordSeparators.Length == 0;
        var ws = status ? new[] { " ", "\r\n", "\n", Environment.NewLine } : wordSeparators;
        return str.Split(ws, StringSplitOptions.RemoveEmptyEntries);
    }

    public static XDocument ToXDocument(this string xml)
    {
        Encoding encoding = Activator.CreateInstance<UTF8Encoding>();
        using var ms = new MemoryStream(encoding.GetBytes(xml));
        return XDocument.Load(ms);
    }

    public static XDocument ToXDocument<TEncoding>(this string xml) where TEncoding : Encoding
    {
        Encoding encoding = Activator.CreateInstance<TEncoding>();
        using var ms = new MemoryStream(encoding.GetBytes(xml));
        return XDocument.Load(ms);
    }

    public static XElement ToXElement(this string xml)
    {
        return XElement.Parse(xml);
    }

    public static string ToXmlAttributeString(this string text)
    {
        var attr = new XAttribute("x", text).ToString();
        var val = attr.Substring(2).Trim('\"');
        return val;
    }

    public static XmlDocument ToXmlDocument(this string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    public static XmlElement? ToXmlElement(this string xml)
    {
        if (xml == null) throw new ArgumentNullException(nameof(xml));
        if (string.IsNullOrEmpty(xml)) throw new ArgumentException("Value cannot be null or empty.", nameof(xml));

        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc.DocumentElement;
    }

    public static string Trim(this string s, string sub, StringComparison comparison = StringComparison.Ordinal)
    {
        return s.TrimStart(sub, comparison).TrimEnd(sub, comparison);
    }

    public static string TrimEnd(this string s, string sub, StringComparison comparison = StringComparison.Ordinal)
    {
        while (s.EndsWith(sub, comparison))
            s = s.Substring(0, s.Length - sub.Length);

        return s;
    }

    public static string TrimStart(this string s, string sub, StringComparison comparison = StringComparison.Ordinal)
    {
        while (s.StartsWith(sub, comparison))
            s = s.Substring(sub.Length);

        return s;
    }

    public static string? TrimToMaxLength(this string? value, int maxLength)
    {
        return value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }

    public static string? TrimToMaxLength(this string? value, int maxLength, string suffix)
    {
        return value == null || value.Length <= maxLength
            ? value
            : string.Concat(value.Substring(0, maxLength), suffix);
    }

    public static string? Truncate(this string? @this, int maxLength)
    {
        const string suffix = "...";

        if (@this == null || @this.Length <= maxLength) return @this;

        var strLength = maxLength - suffix.Length;
        return @this.Substring(0, strLength) + suffix;
    }

    public static string? Truncate(this string? @this, int maxLength, string suffix)
    {
        if (@this == null || @this.Length <= maxLength) return @this;

        var strLength = maxLength - suffix.Length;
        return @this.Substring(0, strLength) + suffix;
    }

    public static string TruncateEnd(this string original, int maxLength)
    {
        if (original == null) throw new ArgumentNullException(nameof(original));
        if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
        return original.Length <= maxLength ? original : original.Substring(0, maxLength);
    }

    public static string TruncateEnd(this string original, int maxLength, string? suffix)
    {
        if (original == null) throw new ArgumentNullException(nameof(original));
        if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
        if (original.Length <= maxLength)
            return original;
        suffix ??= "";
        var length = maxLength - suffix.Length;
        if (length < 0)
            return suffix.Substring(0, maxLength);
        return original.Substring(0, length) + suffix;
    }

    public static bool TryParseAsUri(this string uriString, out Uri? uri)
    {
        try
        {
            uri = new Uri(uriString);
            return true;
        }
        catch
        {
            uri = null;
            return false;
        }
    }
    public static bool TryParseEnum<T>(this string name, out T result, bool ignoreCase = false)
        where T : struct, Enum
    {
        return Enum.TryParse(name, ignoreCase, out result);
    }

    public static string UrlDecode(this string str)
    {
        return HttpUtility.UrlDecode(str);
    }

    public static string UrlDecode(this string str, Encoding e)
    {
        return HttpUtility.UrlDecode(str, e);
    }

    public static byte[] UrlDecodeToBytes(this string str)
    {
        return HttpUtility.UrlDecodeToBytes(str);
    }

    public static byte[] UrlDecodeToBytes(this string str, Encoding e)
    {
        return HttpUtility.UrlDecodeToBytes(str, e);
    }

    public static string UrlEncode(this string str)
    {
        return HttpUtility.UrlEncode(str);
    }

    public static string UrlEncode(this string str, Encoding e)
    {
        return HttpUtility.UrlEncode(str, e);
    }

    public static byte[] UrlEncodeToBytes(this string str)
    {
        return HttpUtility.UrlEncodeToBytes(str);
    }

    public static byte[] UrlEncodeToBytes(this string str, Encoding e)
    {
        return HttpUtility.UrlEncodeToBytes(str, e);
    }

    public static string UrlPathEncode(this string str)
    {
        return HttpUtility.UrlPathEncode(str);
    }

    private static string SubstringAfter(this string original, int index, int offset)
    {
        if (index < 0)
            return original;
        return original.Substring(index + offset);
    }

    private static string SubstringBefore(this string original, int index)
    {
        if (index < 0)
            return original;
        return original.Substring(0, index);
    }
}