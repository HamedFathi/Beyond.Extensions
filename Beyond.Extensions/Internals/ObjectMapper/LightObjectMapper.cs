// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.Internals.ObjectMapper;

internal class LightObjectMapper : ObjectCopyBase
{
    private readonly Dictionary<string, PropertyMap[]> _maps = new();

    internal override void Copy(object source, object target)
    {
        var sourceType = source.GetType();
        var targetType = target.GetType();

        var key = GetMapKey(sourceType, targetType);
        if (!_maps.ContainsKey(key)) MapTypes(sourceType, targetType);

        var propMap = _maps[key];

        for (var i = 0; i < propMap.Length; i++)
        {
            var prop = propMap[i];
            var sourceValue = prop.SourceProperty?.GetValue(source, null);
            prop.DestinationProperty?.SetValue(target, sourceValue, null);
        }
    }

    internal override void MapTypes(Type source, Type target)
    {
        var key = GetMapKey(source, target);
        if (_maps.ContainsKey(key)) return;
        var props = GetMatchingProperties(source, target);
        _maps.Add(key, props.ToArray());
    }
}