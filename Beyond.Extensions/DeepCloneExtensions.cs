// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.ObjectExtended;

public static partial class ObjectExtensions
{
    private static readonly MethodInfo? CloneMethod =
        typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

    public static T? DeepClone<T>(this T? original)
    {
        return (T?)Copy(original);
    }

    private static object? Copy(this object? originalObject)
    {
        return InternalCopy(originalObject, new Dictionary<object, object?>(new ReferenceEqualityComparer()));
    }

    private static void CopyFields(object? originalObject, IDictionary<object, object?> visited, object cloneObject,
        Type typeToReflect,
        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                    BindingFlags.FlattenHierarchy, Func<FieldInfo, bool>? filter = null)
    {
        foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
        {
            if (filter != null && filter(fieldInfo) == false) continue;
            if (IsPrimitive(fieldInfo.FieldType)) continue;
            var originalFieldValue = fieldInfo.GetValue(originalObject);
            var clonedFieldValue = InternalCopy(originalFieldValue, visited);
            fieldInfo.SetValue(cloneObject, clonedFieldValue);
        }
    }

    private static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0) return;
        var walker = new ArrayTraverse(array);
        do
        {
            action(array, walker.Position);
        } while (walker.Step());
    }

    private static object? InternalCopy(object? originalObject, IDictionary<object, object?> visited)
    {
        if (originalObject == null) return null;
        var typeToReflect = originalObject.GetType();
        if (IsPrimitive(typeToReflect)) return originalObject;
        if (visited.ContainsKey(originalObject)) return visited[originalObject];
        if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
        var cloneObject = CloneMethod?.Invoke(originalObject, null);
        if (typeToReflect.IsArray)
        {
            var arrayType = typeToReflect.GetElementType();
            if (arrayType != null && IsPrimitive(arrayType) && cloneObject is Array clonedArray)
            {
                clonedArray.ForEach((array, indices) =>
                    array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
            }
        }

        visited.Add(originalObject, cloneObject);
        if (cloneObject != null)
        {
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        return null;
    }

    private static bool IsPrimitive(this Type type)
    {
        if (type == typeof(string)) return true;
        return type.IsValueType & type.IsPrimitive;
    }

    private static void RecursiveCopyBaseTypePrivateFields(object? originalObject, IDictionary<object, object?> visited,
        object cloneObject, Type typeToReflect)
    {
        if (typeToReflect.BaseType != null)
        {
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
                BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
        }
    }

    internal class ArrayTraverse
    {
        internal int[] Position;
        private readonly int[] _maxLengths;

        internal ArrayTraverse(Array array)
        {
            _maxLengths = new int[array.Rank];
            for (var i = 0; i < array.Rank; ++i) _maxLengths[i] = array.GetLength(i) - 1;
            Position = new int[array.Rank];
        }

        internal bool Step()
        {
            for (var i = 0; i < Position.Length; ++i)
                if (Position[i] < _maxLengths[i])
                {
                    Position[i]++;
                    for (var j = 0; j < i; j++) Position[j] = 0;
                    return true;
                }

            return false;
        }
    }

    internal class ReferenceEqualityComparer : EqualityComparer<object>
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public override bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public override int GetHashCode(object? obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}