using Beyond.Extensions.Enums;

namespace Beyond.Extensions.Types;
public class JsonComparisonResult
{
    public string Path { get; set; } = null!;
    public JsonComparisonStatus Status { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}