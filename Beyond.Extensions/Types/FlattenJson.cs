namespace Beyond.Extensions.Types;

public class FlattenJson
{
    public string Key { get; set; } = null!;
    public JsonValueKind Kind { get; set; }
    public JsonElement Value { get; set; }
}