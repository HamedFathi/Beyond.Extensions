namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

internal class PropertyFactory : IPathElementFactory
{
    public IPathElement Create(string path, out string newPath)
    {
        var property = Regex.Matches(path, @"^\w+")[0].Value;
        newPath = path.Remove(0, property.Length);
        return new Property(property);
    }

    public bool IsApplicable(string path)
    {
        return Regex.IsMatch(path, @"^\w+");
    }
}