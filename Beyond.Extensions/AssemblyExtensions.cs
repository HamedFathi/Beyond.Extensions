// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.AssemblyExtended;

public static class AssemblyExtensions
{
    public static string GetManifestResourceAsString(this Assembly assembly, string resourceName)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        if (string.IsNullOrEmpty(resourceName))
            throw new ArgumentException("Value cannot be null or empty.", nameof(resourceName));

        var result = string.Empty;
        var resourceFileName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));

        if (string.IsNullOrEmpty(resourceFileName)) return result;

        using var stream = assembly.GetManifestResourceStream(resourceFileName);
        if (stream != null)
        {
            using var reader = new StreamReader(stream);
            result = reader.ReadToEnd();
        }

        return result;
    }

    public static IEnumerable<Assembly> GetReferencedAssemblies(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        var listOfAssemblies = new List<Assembly>();
        listOfAssemblies.AddRange(assembly.GetReferencedAssemblies().Select(Assembly.Load));
        return listOfAssemblies;
    }

    public static IEnumerable<Assembly> WithReferencedAssemblies(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        var listOfAssemblies = new List<Assembly>
        {
            assembly
        };
        listOfAssemblies.AddRange(assembly.GetReferencedAssemblies().Select(Assembly.Load));
        return listOfAssemblies;
    }
}