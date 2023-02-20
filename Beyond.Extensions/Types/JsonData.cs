
using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Types;

public class JsonData
{
    public string Key { get; set; } = null!;
    public object? Value { get; set; }
    public JsonDataValueKind Kind { get; set; }
}