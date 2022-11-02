﻿// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Beyond.Extensions.ObservableCollectionExtended;

public static class ObservableCollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        foreach (var item in collection) oc.Add(item);
    }

    public static void AddRange<T>(this ObservableCollection<T> oc, params T[] collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        foreach (var item in collection) oc.Add(item);
    }

    public static void AddRange<T>(this ObservableCollection<T> oc, ICollection<T> collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        foreach (var item in collection) oc.Add(item);
    }
}