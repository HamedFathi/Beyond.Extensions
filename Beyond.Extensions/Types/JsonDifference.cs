using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Types;

public class JsonDifference
{
    public string Path { get; set; } = null!;
    public JsonChangeType ChangeType { get; set; }
    public string? OldValue { get; set; } 
    public string? NewValue { get; set; } 
}