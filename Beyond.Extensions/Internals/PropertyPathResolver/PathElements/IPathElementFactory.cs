﻿namespace Beyond.Extensions.Internals.PropertyPathResolver.PathElements;

public interface IPathElementFactory
{
    /// <summary>
    /// creates a path element from the given path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="newPath">
    /// outputs the path removed by the bit that was used to create the path element
    /// </param>
    /// <returns></returns>
    IPathElement Create(string path, out string newPath);

    /// <summary>
    /// checks if the factory can create a path element from the given path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool IsApplicable(string path);
}