using System.Runtime.Serialization;

namespace Beyond.Extensions.JsonConverters;

public class StringEnumMemberConverter : JsonConverterFactory
{
    private readonly bool _allowIntegerValues;
    private readonly JsonNamingPolicy _namingPolicy;

    public StringEnumMemberConverter()
        : this(namingPolicy: null, allowIntegerValues: true)
    {
    }

    public StringEnumMemberConverter(JsonNamingPolicy namingPolicy = null, bool allowIntegerValues = true)
        => (_namingPolicy, _allowIntegerValues) = (namingPolicy, allowIntegerValues);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum
            || typeToConvert.IsGenericType && TestNullableEnum(typeToConvert).IsNullableEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        (var isNullableEnum, var underlyingType) = TestNullableEnum(typeToConvert);

        return (JsonConverter)Activator.CreateInstance(
            typeof(StringGenericEnumMemberConverter<>).MakeGenericType(typeToConvert),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { _namingPolicy, _allowIntegerValues, isNullableEnum ? underlyingType : null },
            culture: null)!;
    }

    private static (bool IsNullableEnum, Type UnderlyingType) TestNullableEnum(Type typeToConvert)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
        return (underlyingType?.IsEnum ?? false, underlyingType);
    }

    internal class StringGenericEnumMemberConverter<T> : JsonConverter<T>
    {
        private const BindingFlags EnumBindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        private readonly bool _allowIntegerValues;
        private readonly Type _enumType;
        private readonly TypeCode _enumTypeCode;
        private readonly bool _isFlags;
        private readonly Dictionary<ulong, EnumInfo> _rawToTransformed;
        private readonly Dictionary<string, EnumInfo> _transformedToRaw;
        private readonly Type _underlyingType;

        public StringGenericEnumMemberConverter(JsonNamingPolicy namingPolicy, bool allowIntegerValues, Type underlyingType)
        {
            _allowIntegerValues = allowIntegerValues;
            _underlyingType = underlyingType;

            _enumType = _underlyingType ?? typeof(T);
            _enumTypeCode = Type.GetTypeCode(_enumType);
            _isFlags = _enumType.IsDefined(typeof(FlagsAttribute), true);

            var builtInNames = _enumType.GetEnumNames();
            var builtInValues = _enumType.GetEnumValues();

            _rawToTransformed = new Dictionary<ulong, EnumInfo>();
            _transformedToRaw = new Dictionary<string, EnumInfo>();

            for (var i = 0; i < builtInNames.Length; i++)
            {
                var enumValue = (Enum)builtInValues.GetValue(i)!;
                var rawValue = GetEnumValue(enumValue);

                var name = builtInNames[i];
                var field = _enumType.GetField(name, EnumBindings)!;
                var enumMemberAttribute = field.GetCustomAttribute<EnumMemberAttribute>(true);
                var transformedName = enumMemberAttribute?.Value ?? namingPolicy?.ConvertName(name) ?? name;

                _rawToTransformed[rawValue] = new EnumInfo(transformedName, enumValue, rawValue);
                _transformedToRaw[transformedName] = new EnumInfo(name, enumValue, rawValue);
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var token = reader.TokenType;
            if (token == JsonTokenType.String)
            {
                var enumString = reader.GetString()!;

                // Case sensitive search attempted first.
                if (_transformedToRaw.TryGetValue(enumString, out var enumInfo))
                {
                    return (T)Enum.ToObject(_enumType, enumInfo.RawValue);
                }

                if (_isFlags)
                {
                    var calculatedValue = 0UL;

                    var flagValues = enumString.Split(new[] { ", " }, StringSplitOptions.None);
                    foreach (var flagValue in flagValues)
                    {
                        // Case sensitive search attempted first.
                        if (_transformedToRaw.TryGetValue(flagValue, out enumInfo))
                        {
                            calculatedValue |= enumInfo.RawValue;
                        }
                        else
                        {
                            // Case insensitive search attempted second.
                            var matched = false;
                            foreach (var enumItem in _transformedToRaw)
                            {
                                if (string.Equals(enumItem.Key, flagValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    calculatedValue |= enumItem.Value.RawValue;
                                    matched = true;
                                    break;
                                }
                            }

                            if (!matched)
                            {
                                throw new JsonException($"Unknown flag value {flagValue}.");
                            }
                        }
                    }

                    return (T)Enum.ToObject(_enumType, calculatedValue);
                }

                // Case insensitive search attempted second.
                foreach (var enumItem in _transformedToRaw)
                {
                    if (string.Equals(enumItem.Key, enumString, StringComparison.OrdinalIgnoreCase))
                    {
                        return (T)Enum.ToObject(_enumType, enumItem.Value.RawValue);
                    }
                }

                throw new JsonException($"Unknown value {enumString}.");
            }

            if (token != JsonTokenType.Number || !_allowIntegerValues)
            {
                throw new JsonException();
            }

            var result = _enumTypeCode switch
            {
                TypeCode.Int32 when reader.TryGetInt32(out var int32) => (T)Enum.ToObject(_enumType, int32),
                TypeCode.UInt32 when reader.TryGetUInt32(out var uint32) => (T)Enum.ToObject(_enumType, uint32),
                TypeCode.UInt64 when reader.TryGetUInt64(out var uint64) => (T)Enum.ToObject(_enumType, uint64),
                TypeCode.Int64 when reader.TryGetInt64(out var int64) => (T)Enum.ToObject(_enumType, int64),
                TypeCode.SByte when reader.TryGetSByte(out var byte8) => (T)Enum.ToObject(_enumType, byte8),
                TypeCode.Byte when reader.TryGetByte(out var ubyte8) => (T)Enum.ToObject(_enumType, ubyte8),
                TypeCode.Int16 when reader.TryGetInt16(out var int16) => (T)Enum.ToObject(_enumType, int16),
                TypeCode.UInt16 when reader.TryGetUInt16(out var uint16) => (T)Enum.ToObject(_enumType, uint16),
                _ => throw new JsonException()
            };

            return result;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Note: There is no check for value == null because Json serializer won't call the
            // converter in that case.
            var rawValue = GetEnumValue(value!);

            if (_rawToTransformed.TryGetValue(rawValue, out var enumInfo))
            {
                writer.WriteStringValue(enumInfo.Name);
                return;
            }

            if (_isFlags)
            {
                var calculatedValue = 0UL;

                var builder = new StringBuilder();
                foreach (var enumItem in _rawToTransformed)
                {
                    enumInfo = enumItem.Value;
                    if (!(value as Enum)!.HasFlag(enumInfo.EnumValue)
                        || enumInfo.RawValue == 0) // Definitions with 'None' should hit the cache case.
                    {
                        continue;
                    }

                    // Track the value to make sure all bits are represented.
                    calculatedValue |= enumInfo.RawValue;

                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(enumInfo.Name);
                }

                if (calculatedValue == rawValue)
                {
                    writer.WriteStringValue(builder.ToString());
                    return;
                }
            }

            if (!_allowIntegerValues)
            {
                throw new JsonException();
            }

            switch (_enumTypeCode)
            {
                case TypeCode.Int32:
                    writer.WriteNumberValue((int)rawValue);
                    break;

                case TypeCode.UInt32:
                    writer.WriteNumberValue((uint)rawValue);
                    break;

                case TypeCode.UInt64:
                    writer.WriteNumberValue(rawValue);
                    break;

                case TypeCode.Int64:
                    writer.WriteNumberValue((long)rawValue);
                    break;

                case TypeCode.Int16:
                    writer.WriteNumberValue((short)rawValue);
                    break;

                case TypeCode.UInt16:
                    writer.WriteNumberValue((ushort)rawValue);
                    break;

                case TypeCode.Byte:
                    writer.WriteNumberValue((byte)rawValue);
                    break;

                case TypeCode.SByte:
                    writer.WriteNumberValue((sbyte)rawValue);
                    break;

                default:
                    throw new JsonException();
            }
        }

        private ulong GetEnumValue(object value)
            => _enumTypeCode switch
            {
                TypeCode.Int32 => (ulong)(int)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                TypeCode.Int64 => (ulong)(long)value,
                TypeCode.SByte => (ulong)(sbyte)value,
                TypeCode.Byte => (byte)value,
                TypeCode.Int16 => (ulong)(short)value,
                TypeCode.UInt16 => (ushort)value,
                _ => throw new JsonException(),
            };

        private class EnumInfo
        {
            public Enum EnumValue;
            public string Name;
            public ulong RawValue;

            public EnumInfo(string name, Enum enumValue, ulong rawValue)
                => (Name, EnumValue, RawValue) = (name, enumValue, rawValue);
        }
    }
}