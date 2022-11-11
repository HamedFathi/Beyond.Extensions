using Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

namespace Beyond.Extensions.Internals.PropertyPathResolver;

public class Resolver : IResolver
{
    private IList<IPathElementFactory>? _pathElementFactories;

    /// <summary>
    /// contains the path element factories used to resolve given paths
    /// more specific factories must be before more generic ones, because the first applicable one is taken
    /// </summary>
    public IList<IPathElementFactory>? PathElementFactories
    {
        get => _pathElementFactories;
        set => _pathElementFactories = value ?? throw new ArgumentNullException($"The {nameof(PathElementFactories)} must not be null");
    }

    public Resolver()
    {
        PathElementFactories = new List<IPathElementFactory>
        {
            new PropertyFactory(),
            new EnumerableAccessFactory(),
            new DictionaryAccessFactory(),
            new SelectionFactory()
        };
    }

    public Resolver(IPathElementFactory[] pathElementFactories, bool addDefaults = true)
    {
        if (pathElementFactories == null) throw new ArgumentNullException(nameof(pathElementFactories));
        if (pathElementFactories.Length == 0)
            throw new ArgumentException("Value cannot be an empty collection.", nameof(pathElementFactories));

        var list = new List<IPathElementFactory>
        {
            new PropertyFactory(),
            new EnumerableAccessFactory(),
            new DictionaryAccessFactory(),
            new SelectionFactory()
        };

        if (addDefaults)
            list.AddRange(pathElementFactories);

        PathElementFactories = addDefaults ? list : pathElementFactories;
    }

    public IList<IPathElement> CreatePath(string path)
    {
        var pathElements = new List<IPathElement>();
        var tempPath = path;
        while (tempPath.Length > 0)
        {
            var pathElement = CreatePathElement(tempPath, out tempPath);
            pathElements.Add(pathElement);
            //remove the dots chaining properties 
            //no PathElement could do this reliably
            //the only appropriate one would be Property, but there doesn't have to be a dot at the beginning (if it is the first PathElement, e.g. "Property1.Property2")
            //and I don't want that knowledge in PropertyFactory
            if (tempPath.StartsWith("."))
                tempPath = tempPath.Remove(0, 1);
        }
        return pathElements;
    }

    public object? Resolve(object? target, string path)
    {
        var pathElements = CreatePath(path);
        return Resolve(target, pathElements);
    }

    public object? Resolve(object? target, IList<IPathElement> pathElements)
    {
        var tempResult = target;
        foreach (var pathElement in pathElements)
        {
            tempResult = tempResult is Selection selection ? pathElement.Apply(selection) : pathElement.Apply(tempResult);
        }

        var result = tempResult;
        if (result is Selection selection1)
            return selection1.AsEnumerable();
        return result;
    }

    private IPathElement CreatePathElement(string path, out string newPath)
    {
        //get the first applicable path element type
        var pathElementFactory = PathElementFactories?.FirstOrDefault(f => f.IsApplicable(path));

        if (pathElementFactory == null)
            throw new InvalidOperationException($"There is no applicable path element factory for {path}");

        return pathElementFactory.Create(path, out newPath);
    }

    public object? ResolveSafe(object? target, IList<IPathElement> pathElements)
    {
        try
        {
            return Resolve(target, pathElements);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public object? ResolveSafe(object? target, string path)
    {
        try
        {
            return Resolve(target, path);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }
}