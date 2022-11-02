﻿// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.RefExtended;

public static class RefExtensions
{
    /*
    var list = new List<PointStruct>() {
            new PointStruct(1, 10),
            new PointStruct(2, 20),
            new PointStruct(3, 30)
    };
    list.ForEachRef(static (ref PointStruct p) => p.Swap());
    foreach (var p in list) { Console.WriteLine(p); }
    */

    public delegate void RefAction<T>(ref T value) where T : struct;

    public static void ForEachRef<T>(this List<T> list, RefAction<T> action) where T : struct
    {
        if (list is null) throw new ArgumentNullException(nameof(list));
        if (action is null) throw new ArgumentNullException(nameof(action));
        var span = CollectionsMarshal.AsSpan(list);
        foreach (ref var item in span)
            action(ref item);
    }
}