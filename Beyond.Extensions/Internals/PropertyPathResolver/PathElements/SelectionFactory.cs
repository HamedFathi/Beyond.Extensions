namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

internal class SelectionFactory : IPathElementFactory
{
    private const string SelectionIndicator = "[]";

    public IPathElement Create(string path, out string newPath)
    {
        newPath = path.Remove(0, SelectionIndicator.Length);
        return new SelectionAccess();
    }

    public bool IsApplicable(string path)
    {
        return path.StartsWith(SelectionIndicator);
    }
}