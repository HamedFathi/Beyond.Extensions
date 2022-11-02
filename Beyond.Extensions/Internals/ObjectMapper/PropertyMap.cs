// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Beyond.Extensions.Internals.ObjectMapper;

internal class PropertyMap
{
    internal PropertyInfo? DestinationProperty { get; set; }
    internal PropertyInfo? SourceProperty { get; set; }
}