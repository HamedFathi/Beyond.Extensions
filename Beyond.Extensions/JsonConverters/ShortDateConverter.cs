namespace Beyond.Extensions.JsonConverters;

public class ShortDateConverter : JsonConverter<DateTime>
{
    private readonly string _serializationFormat;

    public ShortDateConverter() : this(null)
    {
    }

    public ShortDateConverter(string serializationFormat)
    {
        _serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateTimeOffset.Parse(value!, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal).Date;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat));
}