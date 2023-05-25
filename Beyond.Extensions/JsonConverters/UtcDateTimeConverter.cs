﻿namespace Beyond.Extensions.JsonConverters;

public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _serializationFormat;

    public UtcDateTimeConverter() : this(null)
    {
    }

    public UtcDateTimeConverter(string serializationFormat)
    {
        this._serializationFormat = serializationFormat ?? "yyyy-MM-ddTHH:mm:ss.fffffffZ";
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetDateTime().ToUniversalTime();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue((value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value)
            .ToString(_serializationFormat));
}