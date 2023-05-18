namespace Beyond.Extensions.Types;

public class FlattenJson
{
    public string Key { get; set; } = null!;
    public JsonElement Value { get; set; }
    public JsonValueKind Kind { get; set; }
}