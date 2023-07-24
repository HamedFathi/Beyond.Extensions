// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using Beyond.Extensions.Internals.ObjectMapper;
using Beyond.Extensions.Internals.PropertyPathResolver;
using Beyond.Extensions.TypeExtended;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.ObjectExtended;

public static partial class ObjectExtensions
{
    // Concurrent dictionary cache to store compiled functions for property access
    private static readonly ConcurrentDictionary<string, Func<object, object>> GetCache = new();

    // Concurrent dictionary cache to store compiled functions for property assignment
    private static readonly ConcurrentDictionary<string, Action<object, object>?> SetCache = new();

    public static PropertyAccessor<T> AccessProperty<T>(this T obj, string propertyName)
    {
        var _ = obj;
        return new PropertyAccessor<T>(propertyName);
    }

    public static StaticPropertyAccessor<T> AccessStaticProperty<T>(this T obj, string propertyName)
    {
        var _ = obj;
        return new StaticPropertyAccessor<T>(propertyName);
    }

    public static IEnumerable<T> Across<T>(this T first, Func<T, T?> next) where T : class
    {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (next == null) throw new ArgumentNullException(nameof(next));

        for (var item = first; item != null; item = next(item))
            yield return item;
    }

    public static bool Any<T>(this T obj, params T[] values)
    {
        return Array.IndexOf(values, obj) != -1;
    }

    public static T? As<T>(this object o) where T : class => o as T;

    [SuppressMessage("Roslynator", "RCS1175:Unused 'this' parameter.", Justification = "<Pending>")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static Lazy<T> AsLazy<T>(this T obj)
    {
        return new Lazy<T>();
    }

    public static bool CanConvertTo<T>(this object? value)
    {
        if (value != null)
        {
            var targetType = typeof(T);

            var converter = TypeDescriptor.GetConverter(value);
            if (converter.CanConvertTo(targetType))
                return true;

            converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(value.GetType()))
                return true;
        }

        return false;
    }

    public static T CastAs<T>(this object @this)
    {
        return (T)@this;
    }

    public static T? CastAsOrDefault<T>(this object @this)
    {
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static T CastAsOrDefault<T>(this object @this, T defaultValue)
    {
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static T CastAsOrDefault<T>(this object @this, Func<T> defaultValueFactory)
    {
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValueFactory();
        }
    }

    public static T CastAsOrDefault<T>(this object @this, Func<object, T> defaultValueFactory)
    {
        try
        {
            return (T)@this;
        }
        catch (Exception)
        {
            return defaultValueFactory(@this);
        }
    }

    public static T CastTo<T>(this object o) => (T)o;

    public static object ChangeType(this object value, TypeCode typeCode)
    {
        return Convert.ChangeType(value, typeCode);
    }

    public static object ChangeType(this object value, TypeCode typeCode, IFormatProvider provider)
    {
        return Convert.ChangeType(value, typeCode, provider);
    }

    public static object ChangeType(this object value, Type conversionType)
    {
        return Convert.ChangeType(value, conversionType);
    }

    public static object ChangeType(this object value, Type conversionType, IFormatProvider provider)
    {
        return Convert.ChangeType(value, conversionType, provider);
    }

    public static object ChangeType<T>(this object value)
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }

    public static object ChangeType<T>(this object value, IFormatProvider provider)
    {
        return (T)Convert.ChangeType(value, typeof(T), provider);
    }

    public static T? Coalesce<T>(this T? @this, params T?[] values) where T : class
    {
        if (@this != null) return @this;

        foreach (var value in values)
            if (value != null)
                return value;

        return null;
    }

    public static T? CoalesceOrDefault<T>(this T? @this, params T?[] values) where T : class
    {
        if (@this != null) return @this;

        foreach (var value in values)
            if (value != null)
                return value;

        return default;
    }

    public static T? CoalesceOrDefault<T>(this T? @this, Func<T?> defaultValueFactory, params T?[] values)
        where T : class
    {
        if (@this != null) return @this;

        foreach (var value in values)
            if (value != null)
                return value;

        return defaultValueFactory();
    }

    public static T? ConvertTo<T>(this object value)
    {
        return value.ConvertTo(default(T));
    }

    public static T? ConvertTo<T>(this object? value, T? defaultValue)
    {
        if (value != null)
        {
            var targetType = typeof(T);

            if (value.GetType() == targetType) return (T)value;

            var converter = TypeDescriptor.GetConverter(value);
            if (converter.CanConvertTo(targetType))
                return (T?)converter.ConvertTo(value, targetType);

            converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(value.GetType()))
                return (T?)converter.ConvertFrom(value);
        }

        return defaultValue;
    }

    public static T? ConvertTo<T>(this object value, T? defaultValue, bool ignoreException)
    {
        if (!ignoreException) return value.ConvertTo<T>();
        try
        {
            return value.ConvertTo<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T? ConvertToAndIgnoreException<T>(this object value)
    {
        return value.ConvertToAndIgnoreException(default(T));
    }

    public static T? ConvertToAndIgnoreException<T>(this object value, T? defaultValue)
    {
        return value.ConvertTo(defaultValue, true);
    }

    public static T[] CreateArray<T>(this T obj)
    {
        return new[] { obj };
    }

    public static ICollection<T> CreateCollection<T>(this T obj)
    {
        return CreateList(obj);
    }

    public static IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(this TValue value, TKey key)
        where TKey : notnull
    {
        return new Dictionary<TKey, TValue> { { key, value } };
    }

    public static IEnumerable<T> CreateEnumerable<T>(this T obj)
    {
        yield return obj;
    }

    public static IList<T> CreateList<T>(this T obj)
    {
        return new List<T> { obj };
    }

    public static IReadOnlyCollection<T> CreateReadOnlyCollection<T>(this T obj)
    {
        var result = new ReadOnlyCollection<T>(obj.CreateList());
        return result;
    }

    public static bool Equals<T, TResult>([DisallowNull] this T obj, object obj1, Func<T, TResult> selector)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (obj1 == null) throw new ArgumentNullException(nameof(obj1));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        return obj1 is T obj2 && selector(obj).Equals(selector(obj2));
    }

    public static TReturn? GetNestedPropertyValue<T, TReturn>([DisallowNull] this T obj, string propertyNestedPath)
    {
        /*
        Property
        Property1.Property2
        ArrayProperty[5]
        DictionaryProperty['Key']
        [0] //just an array index
        ['Key'] //just a dictionary index
        NestedArray2
        NestedDictionary['Key1']['Key2']

        NOTE: For Dictionary you should use single quote '' but for Enumerable not.
        */
        return (TReturn?)obj.GetNestedPropertyValue(propertyNestedPath);
    }

    public static object? GetNestedPropertyValue<T>([DisallowNull] this T? obj, string propertyNestedPath)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (propertyNestedPath == null) throw new ArgumentNullException(nameof(propertyNestedPath));
        if (string.IsNullOrEmpty(propertyNestedPath))
            throw new ArgumentException("Value cannot be null or empty.", nameof(propertyNestedPath));
        if (string.IsNullOrWhiteSpace(propertyNestedPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyNestedPath));

        IResolver resolver = new Resolver();
        return resolver.Resolve(obj, propertyNestedPath);
    }

    public static object GetPropertyValue(this object obj, string propertyPath, bool printable = false)
    {
        // Throws an ArgumentNullException if the object is null.
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        // Throws an ArgumentNullException if the propertyPath is null or white space.
        if (string.IsNullOrWhiteSpace(propertyPath)) throw new ArgumentNullException(nameof(propertyPath));

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Constructing a cache key.
        var cacheKey = $"{obj.GetType().FullName}.{propertyPath}";

        // If we've already compiled and cached a function for this type/propertyPath, use it.
        if (GetCache.TryGetValue(cacheKey, out var func))
        {
            // Returns the value of the property. If printable is true, it returns a formatted JSON string.
            return printable ? JsonSerializer.Serialize(func(obj), options) : func(obj);
        }

        // Get the object type and set up a parameter for the expression tree.
        var type = obj.GetType();
        var arg = Expression.Parameter(typeof(object), "x");
        Expression expr = Expression.Convert(arg, type);

        // Looping over each property in the property path.
        foreach (var propertyName in propertyPath.Split('.'))
        {
            // If the property name contains an index (like in an array or list).
            if (propertyName.Contains("["))
            {
                // If the index is not at the start of the property name, get the property without
                // the index.
                if (propertyName.IndexOf('[') > 0)
                {
                    var propertyWithoutIndex = propertyName[..propertyName.IndexOf('[')];
                    var propertyInfo = type?.GetProperty(propertyWithoutIndex) ?? throw new InvalidOperationException();
                    expr = Expression.Property(expr, propertyInfo);
                    type = propertyInfo.PropertyType;
                }

                // The regular expression pattern to extract the index from the property name.
                const string pattern = @"\[(.*?)\]";
                var match = Regex.Match(propertyName, pattern);
                var indexValue = match.Groups[1].Value;

                // If the index is an integer, handle array and collection types.
                if (int.TryParse(indexValue, out var arrayIndex))
                {
                    // If the type is an array.
                    if (type is { IsArray: true })
                    {
                        expr = Expression.ArrayIndex(expr, Expression.Constant(arrayIndex));
                        type = type.GetElementType();
                    }
                    // If the type is a collection.
                    else if (type != null && type.IsCollection())
                    {
                        expr = Expression.Call(expr, type.GetMethod("get_Item") ?? throw new InvalidOperationException(), Expression.Constant(arrayIndex));
                        type = type.GenericTypeArguments[0];
                    }
                    else
                    {
                        throw new NotSupportedException($"Property '{propertyName}' is not an array or list.");
                    }
                }
                // If the type is a dictionary.
                else if (type != null && type.IsDictionary())
                {
                    var propertyInfo2 = type.GetProperty("Item");
                    var keyType = type.GetGenericArguments()[0];

                    // Parsing complex Dictionary keys
                    object? keyValue;
                    if (keyType.IsPrimitive || keyType == typeof(string))
                    {
                        keyValue = Convert.ChangeType(indexValue, keyType);
                    }
                    else
                    {
                        keyValue = keyType.GetMethod("Parse", new[] { typeof(string) })?.Invoke(null, new object[] { indexValue });
                    }

                    if (propertyInfo2 == null) continue;
                    expr = Expression.Property(expr, propertyInfo2, Expression.Constant(keyValue));
                    type = propertyInfo2.PropertyType;
                }
            }
            // If the property is a normal nested property.
            else
            {
                var propertyInfo = type?.GetProperty(propertyName);
                if (propertyInfo == null) continue;
                expr = Expression.Property(expr, propertyInfo);
                type = propertyInfo.PropertyType;
            }
        }

        // Build and compile the lambda expression for the property accessor.
        var lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(expr, typeof(object)), arg);
        func = lambda.Compile();

        // Add the compiled function to the cache.
        GetCache.TryAdd(cacheKey, func);

        // Returns the value of the property. If printable is true, it returns a formatted JSON string.
        return printable ? JsonSerializer.Serialize(func(obj), options) : func(obj);
    }

    public static TypeCode GetTypeCode(this object value)
    {
        return Convert.GetTypeCode(value);
    }

    public static string? GetTypeFullName(this object? @this)
    {
        return @this == null ? string.Empty : @this.GetType().FullName;
    }

    public static string GetTypeName(this object? @this)
    {
        return @this == null ? string.Empty : @this.GetType().Name;
    }

    public static TResult? GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func)
    {
        try
        {
            return func(@this);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static TResult GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func, TResult defaultValue)
    {
        try
        {
            return func(@this);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public static TResult GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func,
        Func<TResult> defaultValueFactory)
    {
        try
        {
            return func(@this);
        }
        catch (Exception)
        {
            return defaultValueFactory();
        }
    }

    public static TResult GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func,
        Func<T, TResult> defaultValueFactory)
    {
        try
        {
            return func(@this);
        }
        catch (Exception)
        {
            return defaultValueFactory(@this);
        }
    }

    public static T IfNotNull<T>(this T obj, Action<T> action)
    {
        if (obj != null) action(obj);

        return obj;
    }

    public static T IfNotNull<T>(this T obj, Action action)
    {
        if (obj != null) action();

        return obj;
    }

    public static TResult? IfNotNull<T, TResult>(this T @this, Func<T, TResult> func)
    {
        return @this != null ? func(@this) : default;
    }

    public static TResult IfNotNull<T, TResult>(this T @this, Func<T, TResult> func, TResult defaultValue)
    {
        return @this != null ? func(@this) : defaultValue;
    }

    public static TResult IfNotNull<T, TResult>(this T @this, Func<T, TResult> func, Func<TResult> defaultValueFactory)
    {
        return @this != null ? func(@this) : defaultValueFactory();
    }

    public static void IfNotType<T>(this object item, Action<T> action) where T : class
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (action == null) throw new ArgumentNullException(nameof(action));

        if (item is not T) action((T)item);
    }

    public static T IfNull<T>(this T obj, Action action)
    {
        if (obj == null) action();

        return obj;
    }

    public static T IfNull<T>(this T obj, Func<T> func)
    {
        return obj == null ? func() : obj;
    }

    public static void IfType<T>(this object item, Action<T> action) where T : class
    {
        if (item is T item1) action(item1);
    }

    public static bool In<T>(this T @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer)
    {
        return items != null && items.Contains(@this, comparer);
    }

    public static bool In<T>(this T @this, params T[] values)
    {
        return Array.IndexOf(values, @this) != -1;
    }

    public static bool In<T>(this T @this, IEnumerable<T>? items)
    {
        return items != null && items.Contains(@this);
    }

    public static bool InRange<T>(this T @this, T minValue, T maxValue) where T : IComparable<T>
    {
        return @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;
    }

    public static object? InvokeMethod(this object obj, string methodName, params object[] parameters)
    {
        return InvokeMethod<object>(obj, methodName, parameters);
    }

    public static T? InvokeMethod<T>(this object obj, string methodName, params object[] parameters)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName, parameters.Select(o => o.GetType()).ToArray());

        if (method == null)
            throw new ArgumentException($"Method '{methodName}' not found.", methodName);

        var value = method.Invoke(obj, parameters);
        return value is T value1 ? value1 : default;
    }

    public static bool IsAnonymousType(this object? instance)
    {
        if (instance == null)
        {
            return false;
        }

        return instance.GetType().Namespace == null;
    }

    public static bool IsArray(this object? @this)
    {
        return @this != null && @this.GetType().IsArray;
    }

    public static bool IsAssignableFrom<T>(this object @this)
    {
        var type = @this.GetType();
        return type.IsAssignableFrom(typeof(T));
    }

    public static bool IsAssignableFrom(this object @this, Type targetType)
    {
        var type = @this.GetType();
        return type.IsAssignableFrom(targetType);
    }

    public static bool IsAssignableTo<T>(this object obj)
    {
        return obj.IsAssignableTo(typeof(T));
    }

    public static bool IsAssignableTo(this object obj, Type type)
    {
        var objectType = obj.GetType();
        return type.IsAssignableFrom(objectType);
    }

    public static bool IsBetween<T>(this T value, T minValue, T maxValue) where T : IComparable<T>
    {
        return minValue.CompareTo(value) == -1 && value.CompareTo(maxValue) == -1;
    }

    public static bool IsBetweenInclusive<T>(this T value, T minValue, T maxValue) where T : IComparable<T>
    {
        return value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0;
    }

    public static bool IsClass(this object? @this)
    {
        return @this != null && @this.GetType().IsClass;
    }

    public static bool IsDate(this object obj)
    {
        return obj.IsTypeOf(typeof(DateTime));
    }

    public static bool IsDBNull<T>(this T value) where T : class
    {
        return Convert.IsDBNull(value);
    }

    public static bool IsDBNull(this object obj)
    {
        return obj.IsTypeOf(typeof(DBNull));
    }

    public static bool IsDefault<T>(this T source)
    {
        return EqualityComparer<T>.Default.Equals(source, default) || source == null;
    }

    public static bool IsEnum(this object? @this)
    {
        return @this != null && @this.GetType().IsEnum;
    }

    public static bool IsGreaterOrEqualsThan<T>(this T value, T compareValue) where T : IComparable<T>
    {
        return value.CompareTo(compareValue) >= 0;
    }

    public static bool IsGreaterThan<T>(this T value, T compareValue) where T : IComparable<T>
    {
        return value.CompareTo(compareValue) > 0;
    }

    public static bool IsNotNull<T>(this T? @this) where T : class
    {
        return @this != null;
    }

    public static bool IsNull<T>(this T? @this) where T : class
    {
        return @this == null;
    }

    public static bool IsNullOrDbNull(this object? @this)
    {
        return @this == null || @this.IsDBNull();
    }

    public static bool IsNumericType(this object o)
    {
        switch (Type.GetTypeCode(o.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;

            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.DBNull:
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.String:
            default:
                return false;
        }
    }

    public static bool IsOfType<T>(this object obj)
    {
        return obj.IsOfType(typeof(T));
    }

    public static bool IsOfType(this object obj, Type type)
    {
        return obj.GetType() == type;
    }

    public static bool IsOfTypeOrInherits<T>(this object obj)
    {
        return obj.IsOfTypeOrInherits(typeof(T));
    }

    public static bool IsOfTypeOrInherits(this object obj, Type type)
    {
        var objectType = obj.GetType();
        do
        {
            if (objectType == type)
                return true;
            if (objectType == objectType.BaseType || objectType.BaseType == null)
                return false;
            objectType = objectType.BaseType;
        } while (true);
    }

    public static bool IsSameType(this object obj1, object obj2)
    {
        return obj1.GetType().IsInstanceOfType(obj2) || obj2.GetType().IsInstanceOfType(obj1);
    }

    public static bool IsSerializable(this object? @this)
    {
        return @this != null && @this.GetType().IsSerializable;
    }

    public static bool IsSmallerOrEqualsThan<T>(this T value, T compareValue) where T : IComparable<T>
    {
        return value.CompareTo(compareValue) <= 0;
    }

    public static bool IsSmallerThan<T>(this T value, T compareValue) where T : IComparable<T>
    {
        return value.CompareTo(compareValue) == -1;
    }

    public static bool IsTypeOf(this object obj, Type type)
    {
        return obj.GetType() == type;
    }

    public static bool IsTypeOf<T>(this object obj)
    {
        return obj.GetType() == typeof(T);
    }

    public static bool IsValueType(this object @this)
    {
        return @this.GetType().IsValueType;
    }

    public static U MapTo<T, U>([DisallowNull] this T source, [DisallowNull] U destination)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (destination == null) throw new ArgumentNullException(nameof(destination));

        var t = source.GetType();
        var u = source.GetType();
        var op = new LightObjectMapper();
        op.MapTypes(t, u);
        return destination;
    }

    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func) where T : notnull
    {
        var t = new Dictionary<T, TResult>();
        return n =>
        {
            if (t.ContainsKey(n))
            {
                return t[n];
            }

            var result = func(n);
            t.Add(n, result);
            return result;
        };
    }

    public static bool None<T>(this T obj, params T[] values)
    {
        return Array.IndexOf(values, obj) == -1;
    }

    public static bool NotIn<T>(this T @this, IEnumerable<T>? items)
    {
        return !@this.In(items);
    }

    public static bool NotIn<T>(this T @this, IEnumerable<T>? items, IEqualityComparer<T>? comparer)
    {
        return !@this.In(items, comparer);
    }

    public static bool NotIn<T>(this T @this, params T[] values)
    {
        return Array.IndexOf(values, @this) == -1;
    }

    public static T? NullIf<T>(this T @this, Func<T, bool> predicate) where T : class
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return predicate(@this) ? null : @this;
    }

    public static T? NullIfEquals<T>(this T @this, T value) where T : class
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        if (value == null) throw new ArgumentNullException(nameof(value));
        return @this.Equals(value) ? null : @this;
    }

    public static T? NullIfEqualsAny<T>(this T @this, params T[] values) where T : class
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));
        if (values == null) throw new ArgumentNullException(nameof(values));
        return Array.IndexOf(values, @this) != -1 ? null : @this;
    }

    public static TOut Return<TIn, TOut>(this TIn value, Func<TIn, TOut> evaluateFunc)
    {
        return evaluateFunc(value);
    }

    public static void SetPropertyValue(this object obj, string propertyPath, object value)
    {
        // Check if the provided object or property path are null, if so, throw an exception
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (string.IsNullOrWhiteSpace(propertyPath)) throw new ArgumentNullException(nameof(propertyPath));

        // Create a unique key for this property setter, based on the type of the object and the
        // property path
        var cacheKey = $"{obj.GetType().FullName}.{propertyPath}";

        // If we've previously created and cached a function to set this property, retrieve it
        if (!SetCache.TryGetValue(cacheKey, out var setter))
        {
            // Get the object's type and set up the parameters for the expression tree
            var type = obj.GetType();
            var arg = Expression.Parameter(typeof(object), "x");
            var valueArg = Expression.Parameter(typeof(object));
            Expression expr = Expression.Convert(arg, type);

            // Loop over each part of the property path, processing one level at a time
            foreach (var propertyName in propertyPath.Split('.'))
            {
                // Check if the property name includes an index (e.g. for accessing an element in a
                // list or array)
                if (propertyName.Contains("["))
                {
                    // If the index is not at the start of the property name, get the property
                    // before the index
                    var propertyWithoutIndex = propertyName[..propertyName.IndexOf('[')];
                    if (!string.IsNullOrEmpty(propertyWithoutIndex))
                    {
                        var propertyInfo = type?.GetProperty(propertyWithoutIndex) ?? throw new InvalidOperationException();
                        expr = Expression.Property(expr, propertyInfo);
                        type = propertyInfo.PropertyType;
                    }

                    // Regular expression to extract the index from the property name
                    const string pattern = @"\[(.*?)\]";
                    var match = Regex.Match(propertyName, pattern);
                    var indexValue = match.Groups[1].Value;

                    if (!int.TryParse(indexValue, out var arrayIndex)) continue;

                    // Handle array and collection types The code here constructs the necessary
                    // expression to set an indexed element
                    if (type is { IsArray: true })
                    {
                        expr = Expression.ArrayAccess(expr, Expression.Constant(arrayIndex));
                        type = type.GetElementType();
                    }
                    else if (type != null && type.IsCollection())
                    {
                        expr = Expression.Property(expr, "Item", Expression.Constant(arrayIndex));
                        type = type.GenericTypeArguments[0];
                    }
                    else
                    {
                        throw new NotSupportedException($"Property '{propertyName}' is not an array or list.");
                    }
                }
                else
                {
                    // If the property is a normal nested property (not an indexed element)
                    var propertyInfo = type?.GetProperty(propertyName);
                    if (propertyInfo == null) throw new InvalidOperationException();
                    expr = Expression.Property(expr, propertyInfo);
                    type = propertyInfo.PropertyType;
                }
            }

            // Convert the provided value to the correct type, compile the expression into a
            // function, and cache it for future use
            if (type != null)
            {
                var convertedValue = Expression.Convert(valueArg, type);
                var assignExpr = Expression.Assign(expr, convertedValue);
                var lambda = Expression.Lambda<Action<object, object>>(assignExpr, arg, valueArg);
                setter = lambda.Compile();
            }

            // Add the compiled function to the cache
            SetCache.TryAdd(cacheKey, setter);
        }

        // Invoke the setter to set the property value
        setter?.Invoke(obj, value);
    }

    public static T? ShallowCopy<T>([DisallowNull] this T @this)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this));

        var method = @this.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        return (T?)method?.Invoke(@this, null);
    }

    public static T[] ToArrayObject<T>(this T obj)
    {
        return new[] { obj };
    }

    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this T item)
    {
        yield return await Task.FromResult(item);
    }

    public static byte[]? ToByteArrayByJsonSerializer(this object? objData)
    {
        if (objData == null)
            return default;
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(objData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        }));
    }

    public static byte[]? ToByteArrayByJsonSerializer<T>(T? objData)
    {
        return objData.ToByteArrayByJsonSerializer();
    }

    public static IEnumerable<object?> ToObjectArray(object? obj)
    {
        if (obj is IEnumerable enumerable)
        {
            return enumerable.Cast<object?>();
        }

        return new[] { obj };
    }

    public static TResult? Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction)
    {
        try
        {
            return tryFunction(@this);
        }
        catch
        {
            return default;
        }
    }

    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue)
    {
        try
        {
            return tryFunction(@this);
        }
        catch
        {
            return catchValue;
        }
    }

    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction,
        Func<TType, TResult> catchValueFactory)
    {
        try
        {
            return tryFunction(@this);
        }
        catch
        {
            return catchValueFactory(@this);
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, out TResult? result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue,
        out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch
        {
            result = catchValue;
            return false;
        }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction,
        Func<TType, TResult> catchValueFactory, out TResult result)
    {
        try
        {
            result = tryFunction(@this);
            return true;
        }
        catch
        {
            result = catchValueFactory(@this);
            return false;
        }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction, Action<TType> catchAction)
    {
        try
        {
            tryAction(@this);
            return true;
        }
        catch
        {
            catchAction(@this);
            return false;
        }
    }

    public static bool TryDispose(this object toDispose)
    {
        var disposable = toDispose as IDisposable;
        if (disposable == null)
            return false;

        disposable.Dispose();
        return true;
    }

    public static bool WhereProperties<T>(this T instance, string search,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) where T : class, new()
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (search == null) throw new ArgumentNullException(nameof(search));

        var props = typeof(T).GetProperties(bindingFlags);
        foreach (var prop in props)
        {
            var val = prop.GetValue(instance, null);
            if (val == null) continue;
            var status = val.ToString()?.IndexOf(search, comparison) >= 0;
            if (status)
            {
                return true;
            }
        }

        return false;
    }

    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }
}