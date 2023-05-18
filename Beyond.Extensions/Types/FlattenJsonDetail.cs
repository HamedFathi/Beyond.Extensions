namespace Beyond.Extensions.Types;

public class FlattenJsonDetail
{
    public string CSharpKind { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string TypeScriptKind { get; set; } = null!;
    public string Value { get; set; } = null!;
    public JsonValueKind ValueKind { get; set; }
}