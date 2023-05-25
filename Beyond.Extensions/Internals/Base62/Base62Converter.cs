// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Internals.Base62;

internal class Base62Converter
{
    private const string DefaultCharacterSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string InvertedCharacterSet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly string _characterSet;

    internal Base62Converter()
    {
        _characterSet = DefaultCharacterSet;
    }

    internal Base62Converter(Base62CharacterSet charset)
    {
        _characterSet = charset == Base62CharacterSet.Default ? DefaultCharacterSet : InvertedCharacterSet;
    }

    internal string Decode(string value)
    {
        var arr = new int[value.Length];
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i] = _characterSet.IndexOf(value[i]);
        }

        return Decode(arr);
    }

    internal string Encode(string value)
    {
        var arr = new int[value.Length];
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i] = value[i];
        }

        return Encode(arr);
    }

    private static int[] BaseConvert(int[] source, int sourceBase, int targetBase)
    {
        var result = new List<int>();
        int count;
        while ((count = source.Length) > 0)
        {
            var quotient = new List<int>();
            var remainder = 0;
            for (var i = 0; i != count; i++)
            {
                var accumulator = source[i] + remainder * sourceBase;
                var digit = accumulator / targetBase;
                remainder = accumulator % targetBase;
                if (quotient.Count > 0 || digit > 0)
                {
                    quotient.Add(digit);
                }
            }

            result.Insert(0, remainder);
            source = quotient.ToArray();
        }

        return result.ToArray();
    }

    private static string Decode(int[] value)
    {
        var converted = BaseConvert(value, 62, 256);
        var builder = new StringBuilder();
        foreach (var t in converted)
        {
            builder.Append((char)t);
        }

        return builder.ToString();
    }

    private string Encode(int[] value)
    {
        var converted = BaseConvert(value, 256, 62);
        var builder = new StringBuilder();
        foreach (var t in converted)
        {
            builder.Append(_characterSet[t]);
        }

        return builder.ToString();
    }
}