namespace Beyond.Extensions.JsonConverters;
// var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
// options.Converters.Add(new DateOnlyConverter()); options.Converters.Add(new TimeOnlyConverter());

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string _serializationFormat;

    public DateOnlyConverter() : this(null)
    {
    }

    public DateOnlyConverter(string serializationFormat)
    {
        _serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override DateOnly Read(ref Utf8JsonReader reader,
        Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat));
}