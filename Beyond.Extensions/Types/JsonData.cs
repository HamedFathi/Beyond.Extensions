using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Types;

public class JsonData
{
    public string Key { get; set; } = null!;
    public JsonDataValueKind Kind { get; set; }
    public object? Value { get; set; }
}