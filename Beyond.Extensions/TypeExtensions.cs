﻿// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using Beyond.Extensions.Enums;
using Beyond.Extensions.ObjectExtended;
using Beyond.Extensions.StringExtended;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.TypeExtended;

public static class TypeExtensions
{
    private static readonly IList<Type> IntegerTypes = new List<Type>
    {
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(sbyte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(byte?),
        typeof(short?),
        typeof(int?),
        typeof(long?),
        typeof(sbyte?),
        typeof(ushort?),
        typeof(uint?),
        typeof(ulong?)
    };

    public static bool CanBeCastTo<T>(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        var destinationType = typeof(T);
        return CanBeCastTo(type, destinationType);
    }

    public static bool CanBeCastTo(this Type? type, Type destinationType)
    {
        if (type == null)
        {
            return false;
        }

        if (type == destinationType)
        {
            return true;
        }

        return destinationType.IsAssignableFrom(type);
    }

    public static T CloseAndBuildAs<T>(this Type openType, params Type[] parameterTypes)
    {
        var closedType = openType.MakeGenericType(parameterTypes);
        return (T)Activator.CreateInstance(closedType)!;
    }

    public static T CloseAndBuildAs<T>(this Type openType, object ctorArgument, params Type[] parameterTypes)
    {
        var closedType = openType.MakeGenericType(parameterTypes);
        return (T)Activator.CreateInstance(closedType, ctorArgument)!;
    }

    public static T CloseAndBuildAs<T>(this Type openType, object ctorArgument1, object ctorArgument2,
        params Type[] parameterTypes)
    {
        var closedType = openType.MakeGenericType(parameterTypes);
        return (T)Activator.CreateInstance(closedType, ctorArgument1, ctorArgument2)!;
    }

    public static bool Closes(this Type? type, Type openType)
    {
        if (type == null)
        {
            return false;
        }

        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == openType)
        {
            return true;
        }

        foreach (var @interface in type.GetInterfaces())
            if (@interface.Closes(openType))
            {
                return true;
            }

        var baseType = typeInfo.BaseType;
        if (baseType == null)
        {
            return false;
        }

        var baseTypeInfo = baseType.GetTypeInfo();
        var closes = baseTypeInfo.IsGenericType && baseType.GetGenericTypeDefinition() == openType;
        if (closes)
        {
            return true;
        }

        return typeInfo.BaseType?.Closes(openType) ?? false;
    }

    public static T Create<T>(this Type type)
    {
        return (T)type.Create();
    }

    public static object Create(this Type type)
    {
        return Activator.CreateInstance(type)!;
    }

    public static T? CreateGenericTypeInstance<T>(this Type genericType, params Type[] typeArguments) where T : class
    {
        var constructedType = genericType.MakeGenericType(typeArguments);
        var instance = Activator.CreateInstance(constructedType);
        return instance as T;
    }

    public static object? CreateInstance(this Type type, params object[] constructorParameters)
    {
        return CreateInstance<object>(type, constructorParameters);
    }

    public static T? CreateInstance<T>(this Type type, params object[] constructorParameters)
    {
        var instance = Activator.CreateInstance(type, constructorParameters);
        return (T?)instance;
    }

    public static LambdaExpression CreateLambdaExpression(this Type type, string propertyName)
    {
        var param = Expression.Parameter(type, "x");
        Expression body = param;
        foreach (var member in propertyName.Split('.')) body = Expression.PropertyOrField(body, member);
        return Expression.Lambda(body, param);
    }

    public static Type? DeriveElementType(this Type type)
    {
        return type.GetElementType() ?? type.GetGenericArguments().FirstOrDefault();
    }

    public static IDictionary<string, int> EnumToDictionary(this Type @this)
    {
        if (@this == null) throw new NullReferenceException();
        if (!@this.IsEnum) throw new InvalidCastException("object is not an Enum.");

        var names = Enum.GetNames(@this);
        var values = Enum.GetValues(@this);

        return (from i in Enumerable.Range(0, names.Length)
                select new { Key = names[i], Value = (int)values.GetValue(i) })
            .ToDictionary(k => k.Key, k => k.Value);
    }

    public static Type? FindInterfaceThatCloses(this Type type, Type openType)
    {
        if (type == typeof(object))
        {
            return null;
        }

        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsInterface && typeInfo.IsGenericType && type.GetGenericTypeDefinition() == openType)
        {
            return type;
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            var interfaceTypeInfo = interfaceType.GetTypeInfo();
            if (interfaceTypeInfo.IsGenericType && interfaceType.GetGenericTypeDefinition() == openType)
            {
                return interfaceType;
            }
        }

        if (!type.IsConcrete())
        {
            return null;
        }

        return typeInfo.BaseType == typeof(object)
            ? null
            : typeInfo.BaseType?.FindInterfaceThatCloses(openType);
    }

    public static IEnumerable<MethodInfo> FindMethodInfo(this Type source, string methodName, int parametersLength, BindingFlags bindingFlags)
    {
        return source.GetMethods(bindingFlags)
                .Where(x => string.Equals(x.Name, methodName, StringComparison.Ordinal) &&
                            x.GetParameters().Length == parametersLength);
    }

    public static IEnumerable<MethodInfo> FindMethodInfo(this Type source, string methodName, int parametersLength)
    {
        return source.GetMethods()
                .Where(x => string.Equals(x.Name, methodName, StringComparison.Ordinal) &&
                            x.GetParameters().Length == parametersLength);
    }

    public static Type? FindParameterTypeTo(this Type type, Type openType)
    {
        var interfaceType = type.FindInterfaceThatCloses(openType);
        return interfaceType?.GetGenericArguments().FirstOrDefault();
    }

    public static void ForAttribute<T>(this Type type, Action<T> action) where T : Attribute
    {
        var attributes = type.GetTypeInfo().GetCustomAttributes(typeof(T));
        foreach (var attribute in attributes)
        {
            var att = (T)attribute;
            action(att);
        }
    }

    public static void ForAttribute<T>(this Type type, Action<T> action, Action elseDo)
        where T : Attribute
    {
        var attributes = type.GetTypeInfo().GetCustomAttributes(typeof(T)).ToArray();
        foreach (var attribute in attributes)
        {
            var att = (T)attribute;
            action(att);
        }

        if (!attributes.Any())
        {
            elseDo();
        }
    }

    public static Type?[] GenericTypeArguments(this Type type)
    {
        switch (type)
        {
            case { IsArray: true }:
                return new[] { type.GetElementType() };

            case { IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(IEnumerable<>):
                return type.GetGenericArguments();

            default:
                {
                    var enumType = type.GetInterfaces()
                        .Where(t => t.IsGenericType &&
                                    t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Select(t => t.GenericTypeArguments).FirstOrDefault();
                    return enumType ?? new[] { type };
                }
        }
    }

    public static IEnumerable<AccessModifierKind> GetAccessModifiers(this Type type)
    {
        var accessModifiers = new List<AccessModifierKind>();

        if (type.IsPublic)
        {
            accessModifiers.Add(AccessModifierKind.Public);
        }
        else if (type.IsNotPublic)
        {
            accessModifiers.Add(AccessModifierKind.Private);
        }
        else if (type.IsNestedFamily)
        {
            accessModifiers.Add(AccessModifierKind.Protected);
        }
        else if (type.IsNestedAssembly)
        {
            accessModifiers.Add(AccessModifierKind.Internal);
        }
        return accessModifiers;
    }

    public static T? GetAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetTypeInfo().GetCustomAttributes<T>().FirstOrDefault();
    }

    public static AttributeTargets GetAttributeTargets(this Type attributeType)
    {
        if (!typeof(Attribute).IsAssignableFrom(attributeType))
        {
            throw new ArgumentException("Specified type must be an Attribute.", nameof(attributeType));
        }
        var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
        if (attributeUsage == null)
        {
            throw new InvalidOperationException($"{attributeType.FullName} does not have an AttributeUsageAttribute.");
        }
        return attributeUsage.ValidOn;
    }

    public static string[] GetAttributeTargetsAsString(this Type attributeType)
    {
        return attributeType.GetAttributeTargets().ToString().Split(',').Select(x => x.Trim()).ToArray();
    }

    public static TValue? GetAttributeValue<TAttribute, TValue>(this Type type, string memberName, Func<TAttribute, TValue> valueSelector, bool inherit = false) where TAttribute : Attribute
    {
        if (type.GetMember(memberName).FirstOrDefault()?.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() is TAttribute att)
        {
            return valueSelector(att);
        }
        return default;
    }

    public static string GetCleanName(this Type typeRef, GenericPresentationMode genericMode = GenericPresentationMode.Normal, bool fullName = false)
    {
        var name = GetCleanedName(typeRef, fullName);

        return genericMode switch
        {
            GenericPresentationMode.Cleaned => Regex.Replace(name, "<.+>", "", RegexOptions.Compiled),
            GenericPresentationMode.Simplified => SimplifyGenerics(name),
            GenericPresentationMode.Normal => name,
            _ => throw new ArgumentOutOfRangeException(nameof(genericMode), genericMode, null)
        };

        static string GetCleanedName(Type typeRef, bool fullName = false)
        {
            var rootType = typeRef.IsGenericType
                ? typeRef.GetGenericTypeDefinition()
                : typeRef;

            var cleanedName = rootType.IsPrimitive
                ? rootType.Name
                : rootType.ToString();

            if (!fullName && typeRef.Namespace != null && cleanedName.StartsWith(typeRef.Namespace))
                cleanedName = cleanedName.Substring(typeRef.Namespace.Length + 1);

            if (!typeRef.IsGenericType)
                return cleanedName;
            return cleanedName
                       .Substring(0, cleanedName.LastIndexOf('`'))
                   + typeRef.GetGenericArguments()
                       .Aggregate("<", (r, i) => r + (r != "<" ? ", " : null) + GetCleanedName(i, fullName)) + ">";
        }
        static string SimplifyGenerics(string text)
        {
            if (!text.Contains('<'))
            {
                return text;
            }
            if (!text.Contains(','))
            {
                text = Regex.Replace(text, "<.+>", "<>", RegexOptions.Compiled);
            }
            else
            {
                while (true)
                {
                    var isMatch1 = Regex.IsMatch(text, "<[^<>,]+?,", RegexOptions.Compiled);
                    var isMatch2 = Regex.IsMatch(text, ",[^<>,]+>", RegexOptions.Compiled);
                    var isMatch3 = Regex.IsMatch(text, ",[^<>,]+?,", RegexOptions.Compiled);

                    if (!isMatch1 && !isMatch2 && !isMatch3) break;

                    text = Regex.Replace(text, "<[^<>,]+?,", "<,", RegexOptions.Compiled);
                    text = Regex.Replace(text, ",[^<>,]+>", ",>", RegexOptions.Compiled);
                    text = Regex.Replace(text, ",[^<>,]+?,", ",,", RegexOptions.Compiled);
                }
            }

            return text.RemoveWhiteSpaces();
        }
    }

    public static Type? GetCoreType(this Type input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        if (!input.GetTypeInfo().IsValueType) return input;

        return Nullable.GetUnderlyingType(input);
    }

    public static IEnumerable<string> GetEnumNames(this Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enumeration.");
        }
        var enumValues = Enum.GetValues(enumType);
        foreach (var val in enumValues)
        {
            yield return val.ToString();
        }
    }

    public static string GetEnumNamesAsString(this Type enumType, string separator = ", ")
    {
        return string.Join(separator, enumType.GetEnumNames());
    }

    public static IEnumerable<long> GetEnumValues(this Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enumeration.");
        }

        var enumValues = Enum.GetValues(enumType);

        foreach (var val in enumValues)
        {
            yield return Convert.ToInt64(val);
        }
    }

    public static string GetEnumValuesAsString(this Type enumType, string separator = ", ")
    {
        return string.Join(separator, enumType.GetEnumValues());
    }

    public static string? GetFullName(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType)
        {
            var parameters = type.GetGenericArguments().Select(x => x.GetName()).ToArray();
            var parameterList = string.Join(", ", parameters);
            return $"{type.Name}<{parameterList}>";
        }

        return type.FullName;
    }

    public static string? GetFullTypeName(this Type? type)
    {
        if (type == null) return string.Empty;
        if (type.IsDotNetSystemType()) return type.FullName;
        return type.Assembly.GetName().GetPublicKeyToken()?.Length <= 0
            ? $"{type.FullName}, {type.Assembly.GetName().Name}"
            : type.AssemblyQualifiedName;
    }

    public static Dictionary<string, List<string>> GetGenericConstraints(this Type type)
    {
        var result = new Dictionary<string, List<string>>();

        foreach (var genericArgument in type.GetGenericArguments())
        {
            var gpa = genericArgument.GenericParameterAttributes;
            var constraints = genericArgument.GetGenericParameterConstraints().Select(constraintType => constraintType.Name).ToList();

            if ((gpa & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
            {
                constraints.Add("new()");
            }

            if ((gpa & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
            {
                constraints.Add("notnull");
            }

            result.Add(genericArgument.Name, constraints);
        }

        return result;
    }

    public static Type GetInnerTypeFromNullable(this Type nullableType)
    {
        return nullableType.GetGenericArguments()[0];
    }

    public static List<Type> GetInnerTypes(this Type type, bool distinct = false)
    {
        var innerTypes = new List<Type>();

        if (type.IsGenericType)
        {
            innerTypes.Add(type);
            innerTypes.Add(type.GetGenericTypeDefinition());
            foreach (var argument in type.GetGenericArguments())
            {
                innerTypes.AddRange(argument.GetInnerTypes(distinct));
            }
        }
        else if (type.IsArray)
        {
            innerTypes.Add(type);
            var innerElementType = type.GetElementType()?.GetInnerTypes(distinct);
            if (innerElementType != null) innerTypes.AddRange(innerElementType);
        }
        else if (type.IsTuple())
        {
            innerTypes.Add(type);
            foreach (var argument in type.GetGenericArguments())
            {
                innerTypes.AddRange(argument.GetInnerTypes(distinct));
            }
        }
        else if ((type.IsValueType || type.IsClass) && type is { IsPrimitive: false, IsInterface: false })
        {
            if ((type.Namespace != null && !type.Namespace.StartsWith("System.")) || type.Namespace == null)
            {
                innerTypes.Add(type);
                foreach (var property in type.GetProperties())
                {
                    innerTypes.AddRange(property.PropertyType.GetInnerTypes(distinct));
                }
            }
        }
        else if (type.IsEnum)
        {
            innerTypes.Add(type);
        }
        else if (type.IsInterface)
        {
            innerTypes.Add(type);
            foreach (var interfaceType in type.GetInterfaces())
            {
                innerTypes.AddRange(interfaceType.GetInnerTypes());
            }
            foreach (var property in type.GetProperties())
            {
                innerTypes.AddRange(property.PropertyType.GetInnerTypes(distinct));
            }
        }
        else
        {
            innerTypes.Add(type);
        }

        return distinct ? innerTypes.Distinct().ToList() : innerTypes.ToList();
    }

    public static TypeNode GetInnerTypesAsTree(this Type type)
    {
        return GetStructuredInnerTypes(type, null);

        static TypeNode GetStructuredInnerTypes(Type type, TypeNode? parent)
        {
            var typeNode = new TypeNode(type);

            parent?.Children.Add(typeNode);

            if (type.IsGenericType)
            {
                typeNode.Children.Add(new TypeNode(type.GetGenericTypeDefinition()));
                foreach (var argument in type.GetGenericArguments())
                {
                    GetStructuredInnerTypes(argument, typeNode);
                }
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType != null)
                    GetStructuredInnerTypes(elementType, typeNode);
            }
            else if (type.IsTuple())
            {
                foreach (var argument in type.GetGenericArguments())
                {
                    GetStructuredInnerTypes(argument, typeNode);
                }
            }
            else if ((type.IsValueType || type.IsClass) && type is { IsPrimitive: false, IsInterface: false })
            {
                if ((type.Namespace != null && !type.Namespace.StartsWith("System.")) || type.Namespace == null)
                {
                    foreach (var property in type.GetProperties())
                    {
                        GetStructuredInnerTypes(property.PropertyType, typeNode);
                    }
                }
            }
            else if (type.IsEnum)
            {
                typeNode.Children.Add(new TypeNode(type));
            }
            else if (type.IsInterface)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    GetStructuredInnerTypes(interfaceType, typeNode);
                }
                foreach (var property in type.GetProperties())
                {
                    GetStructuredInnerTypes(property.PropertyType, typeNode);
                }
            }

            return typeNode;
        }
    }

    public static MethodInfo? GetMethodInfo(this Type source, string methodName, BindingFlags bindingFlags, params Type[] parametersTypes)
    {
        return source.GetMethod(methodName, bindingFlags, parametersTypes);
    }

    public static MethodInfo? GetMethodInfo(this Type source, string methodName, params Type[] parametersTypes)
    {
        return source.GetMethod(methodName, parametersTypes);
    }

    public static string GetName(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType)
        {
            var parameters = type.GetGenericArguments().Select(x => x.GetName()).ToArray();
            var parameterList = string.Join(", ", parameters);
            return $"{type.Name}<{parameterList}>";
        }

        return type.Name;
    }

    public static IEnumerable<Type> GetNestedInterfaces(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var interfaces = type.GetInterfaces();

        var baseType = type.BaseType;
        while (baseType != null)
        {
            interfaces = interfaces.Concat(baseType.GetInterfaces()).ToArray();
            baseType = baseType.BaseType;
        }

        interfaces = interfaces.SelectMany(i => i.GetNestedInterfaces()).Concat(interfaces).ToArray();

        return interfaces.Distinct();
    }

    public static string GetPrettyName(this Type type)
    {
        return GetPrettyNameOf(type, new List<Type>());
    }

    public static ReflectionTypeKind GetReflectionTypeKind(this Type type)
    {
        if (type.IsClass)
        {
            return ReflectionTypeKind.Class;
        }

        if (type.IsInterface)
        {
            return ReflectionTypeKind.Interface;
        }
        if (type.IsEnum)
        {
            return ReflectionTypeKind.Enum;
        }
        if (type.IsValueType)
        {
            return ReflectionTypeKind.ValueType;
        }
        throw new ArgumentException("Invalid type");
    }

    public static bool HasAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetTypeInfo().GetCustomAttributes<T>().Any();
    }

    public static bool HasAttribute<TAttribute>(this PropertyInfo @this, bool inherit = false)
        where TAttribute : Attribute
    {
        var tAttribute = typeof(TAttribute);
        var attrs = @this.GetCustomAttributes(inherit);
        return IsOneOfAttributes(tAttribute, attrs);
    }

    public static bool HasAttribute<TAttribute>(this MemberInfo @this, bool inherit = false)
        where TAttribute : Attribute
    {
        var tAttribute = typeof(TAttribute);
        var attrs = @this.GetCustomAttributes(inherit);
        return IsOneOfAttributes(tAttribute, attrs);
    }

    public static bool HasAttribute<TAttribute>(this FieldInfo @this, bool inherit = false) where TAttribute : Attribute
    {
        var tAttribute = typeof(TAttribute);
        var attrs = @this.GetCustomAttributes(inherit);
        return IsOneOfAttributes(tAttribute, attrs);
    }

    public static bool HasAttribute<TAttribute>(this MethodInfo @this, bool inherit = false)
        where TAttribute : Attribute
    {
        var tAttribute = typeof(TAttribute);
        var attrs = @this.GetCustomAttributes(inherit);
        return IsOneOfAttributes(tAttribute, attrs);
    }

    public static bool HasAttribute<TAttribute>(this EventInfo @this, bool inherit = false) where TAttribute : Attribute
    {
        var tAttribute = typeof(TAttribute);
        var attrs = @this.GetCustomAttributes(inherit);
        return IsOneOfAttributes(tAttribute, attrs);
    }

    public static bool HasAttribute<TAttribute>(this Type @this, bool inherit) where TAttribute : Attribute
    {
        var tAttribute = typeof(TAttribute);
        var attrs = @this.GetCustomAttributes(inherit);
        return IsOneOfAttributes(tAttribute, attrs);
    }

    public static bool HasDefaultConstructor(this Type t)
    {
        return t.IsValueType || t.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, Type.EmptyTypes, null) != null;
    }

    public static bool HasIEnumerable(this Type type)
    {
        return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
    }

    public static bool HasIEnumerableOfT(this Type type)
    {
        return type.GetInterfaces().Any(x => x.IsGenericType
                                             && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool HasIndexer(this Type type)
    {
        return type.GetProperties().Any(p => p.GetIndexParameters().Length > 0);
    }

    public static bool HasInterface(this Type type)
    {
        foreach (var l in GetNestedInterfaces(type))
            if (l.FullName == type.FullName)
                return true;

        return false;
    }

    public static bool HasInterface<T>(this Type type)
    {
        return typeof(T).IsAssignableFrom(type);
    }

    public static bool HasInterface(this Type type, Type interfaceType)
    {
        return interfaceType.IsAssignableFrom(type);
    }

    public static bool HasInterfaces(this Type type, params Type[] interfaceTypes)
    {
        foreach (var interfaceType in interfaceTypes)
            if (!interfaceType.IsAssignableFrom(type))
                return false;
        return true;
    }

    public static bool ImplementsInterfaceTemplate(this Type pluggedType, Type templateType)
    {
        if (!pluggedType.IsConcrete())
        {
            return false;
        }

        foreach (var interfaceType in pluggedType.GetInterfaces())
        {
            if (interfaceType.GetTypeInfo().IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == templateType)
            {
                return true;
            }
        }

        return false;
    }

    public static Type IsAnEnumerationOf(this Type type)
    {
        if (!type.Closes(typeof(IEnumerable<>)))
        {
            throw new Exception("Duh, its gotta be enumerable");
        }

        if (type.IsArray)
        {
            return type.GetElementType()!;
        }

        if (type.GetTypeInfo().IsGenericType)
        {
            return type.GetGenericArguments()[0];
        }

        throw new Exception($"I don't know how to figure out what this is a collection of. Can you tell me? {type}");
    }

    public static bool IsAssignableFrom<T>(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return type.IsAssignableFrom(typeof(T));
    }

    public static bool IsAssignableTo(this Type type, Type? other)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (other == null)
            return false;

        return other.IsAssignableFrom(type);
    }

    public static bool IsAssignableTo<T>(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return type.IsAssignableTo(typeof(T));
    }

    public static bool IsBoolean(this Type typeToCheck)
    {
        return typeToCheck == typeof(bool) || typeToCheck == typeof(bool?);
    }

    public static bool IsClosedTypeOf(this Type @this, Type openGeneric)
    {
        return TypesAssignableFrom(@this).Any(t =>
        {
            if (!t.GetTypeInfo().IsGenericType || @this.GetTypeInfo().ContainsGenericParameters) return false;
            return t.GetGenericTypeDefinition() == openGeneric;
        });
        static IEnumerable<Type> TypesAssignableFrom(Type candidateType)
        {
            return candidateType.GetTypeInfo().ImplementedInterfaces
                .Concat(candidateType.Across(t => t.GetTypeInfo().BaseType));
        }
    }

    public static bool IsCollection(this Type type)
    {
        // return type.GetInterface("ICollection") != null;
        return !type.IsArray && typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    public static bool IsConcrete(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        var typeInfo = type.GetTypeInfo();
        return !typeInfo.IsAbstract && !typeInfo.IsInterface;
    }

    public static bool IsConcreteTypeOf<T>(this Type? pluggedType)
    {
        if (pluggedType == null)
        {
            return false;
        }

        return pluggedType.IsConcrete() && typeof(T).IsAssignableFrom(pluggedType);
    }

    public static bool IsConcreteWithDefaultCtor(this Type type)
    {
        return type.IsConcrete() && type.GetConstructor(Type.EmptyTypes) != null;
    }

    public static bool IsDateTime(this Type typeToCheck)
    {
        return typeToCheck == typeof(DateTime) || typeToCheck == typeof(DateTime?);
    }

    public static bool IsDerived<T>(this Type type)
    {
        var baseType = typeof(T);

        if (baseType.FullName == type.FullName) return true;

        if (type.IsClass)
            return baseType.IsClass
                ? type.IsSubclassOf(baseType)
                : baseType.IsInterface && type.IsImplemented(baseType);
        if (type.IsInterface && baseType.IsInterface) return type.IsImplemented(baseType);
        return false;
    }

    public static bool IsDictionary(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>) && typeof(IDictionary).IsAssignableFrom(type);
    }

    public static bool IsDotNetSystemType(this Type? type)
    {
        if (type == null) return false;
        var nameToCheck = type.Assembly.GetName();
        var exceptions = new[] { "Microsoft.SqlServer.Types" };
        return new[] { "System", "mscorlib", "System.", "Microsoft." }
                   .Any(s => nameToCheck.Name != null &&
                             ((s.EndsWith(".") && nameToCheck.Name.StartsWith(s)) || nameToCheck.Name == s)) &&
               !exceptions.Any(s => nameToCheck.Name != null && nameToCheck.Name.StartsWith(s));
    }

    public static bool IsEnumerable(this Type type, bool excludeString = true)
    {
        if (excludeString && type == typeof(string))
        {
            return false;
        }
        return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
    }

    public static bool IsEnumerableOfT(this Type type)
    {
        return type.GetInterfaces().Any(x => x.IsGenericType
                                             && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool IsFloatingPoint(this Type type)
    {
        return type == typeof(decimal) || type == typeof(float) || type == typeof(double);
    }

    public static bool IsGenericEnumerable(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        var genericArgs = type.GetGenericArguments();
        return genericArgs.Length == 1 && typeof(IEnumerable<>).MakeGenericType(genericArgs).IsAssignableFrom(type);
    }

    public static bool IsGenericType(this Type type)
    {
        return type.GetTypeInfo().IsGenericTypeDefinition;
    }

    public static bool IsGenericTypeDefinedAs(this Type type, Type otherType)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (!type.IsGenericType)
            return false;

        return type.GetGenericTypeDefinition() == otherType;
    }

    public static bool IsIEnumerable(this Type type)
    {
        return type.GetInterfaces().Any(x => x == typeof(IEnumerable));
    }

    public static bool IsIEnumerableOfT(this Type type)
    {
        return type.GetInterfaces().Any(x => x.IsGenericType
                                             && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static bool IsImplemented(this Type type, Type baseType)
    {
        var faces = type.GetInterfaces();
        foreach (var face in faces)
            if (baseType.Name.Equals(face.Name))
                return true;
        return false;
    }

    public static bool IsInNamespace(this Type? type, string nameSpace)
    {
        if (type == null)
        {
            return false;
        }

        return type.Namespace?.StartsWith(nameSpace) ?? false;
    }

    public static bool IsInstanceOfType(this Type type, object? obj)
    {
        return obj != null && type.IsInstanceOfType(obj);
    }

    public static bool IsIntegerBased(this Type type)
    {
        return IntegerTypes.Contains(type);
    }

    public static bool IsNetSystemType(this Type? type)
    {
        if (type == null) return false;
        var nameToCheck = type.Assembly.GetName();
        var exceptions = new[] { "Microsoft.SqlServer.Types" };
        return new[] { "System", "mscorlib", "System.", "Microsoft." }
                   .Any(s => nameToCheck.Name != null &&
                             ((s.EndsWith(".") && nameToCheck.Name.StartsWith(s)) || nameToCheck.Name == s)) &&
               !exceptions.Any(s => nameToCheck.Name != null && nameToCheck.Name.StartsWith(s));
    }

    public static bool IsNotConcrete(this Type type)
    {
        return !type.IsConcrete();
    }

    public static bool IsNullable(this Type input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        return !input.IsValueType ||
               (input.IsGenericType && input.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    public static bool IsNullableOf(this Type theType, Type otherType)
    {
        return theType.IsNullableOfT() && theType.GetGenericArguments()[0] == otherType;
    }

    public static bool IsNullableOfT(this Type? theType)
    {
        if (theType == null)
        {
            return false;
        }

        return theType.GetTypeInfo().IsGenericType && theType.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static bool IsNullableType(this Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    public static bool IsNullableValueType(this Type type)
    {
        if (type is not { IsValueType: true }) return false;
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static bool IsNumeric(this Type type)
    {
        return type.IsFloatingPoint() || type.IsIntegerBased();
    }

    public static bool IsNumericType(this Type t)
    {
        switch (Type.GetTypeCode(t))
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

    public static bool IsOpenGeneric(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        var typeInfo = type.GetTypeInfo();
        return typeInfo.IsGenericTypeDefinition || typeInfo.ContainsGenericParameters;
    }

    public static bool IsPrimitive(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        return typeInfo.IsPrimitive && !IsString(type) && type != typeof(IntPtr);
    }

    public static bool IsQueryable(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>);
    }

    public static bool IsRecord(this Type type)
    {
        var isRecord1 = ((TypeInfo)type).DeclaredProperties.FirstOrDefault(x => x.Name == "EqualityContract")
            ?.GetMethod
            ?.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
        var isRecord2 = type.GetMethod("<Clone>$") != null;
        return isRecord1 || isRecord2;
    }

    public static bool IsSameAsOrSubclassOf(this Type type, Type otherType)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (otherType == null) throw new ArgumentNullException(nameof(otherType));
        return type == otherType || type.IsSubclassOf(otherType);
    }

    public static bool IsSameAsOrSubclassOf<TClass>(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return type.IsSameAsOrSubclassOf(typeof(TClass));
    }

    public static bool IsSimple(this Type type)
    {
        var typeInfo = type.GetTypeInfo();
        return typeInfo.IsPrimitive || IsString(type) || typeInfo.IsEnum;
    }

    public static bool IsStatic(this Type type)
    {
        return type is { IsAbstract: true, IsSealed: true };
    }

    public static bool IsString(this Type type)
    {
        return type == typeof(string);
    }

    public static bool IsSubclassOfRawGeneric(this Type? toCheck, Type baseType)
    {
        while (toCheck != typeof(object))
        {
            var cur = toCheck is { IsGenericType: true } ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (baseType == cur) return true;

            toCheck = toCheck?.BaseType;
        }

        return false;
    }

    public static bool IsTuple(this Type type)
    {
        if (!type.IsGenericType) return false;
        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(Tuple<>) ||
               genericTypeDefinition == typeof(Tuple<,>) ||
               genericTypeDefinition == typeof(Tuple<,,>) ||
               genericTypeDefinition == typeof(Tuple<,,,>) ||
               genericTypeDefinition == typeof(Tuple<,,,,>) ||
               genericTypeDefinition == typeof(Tuple<,,,,,>) ||
               genericTypeDefinition == typeof(Tuple<,,,,,,>) ||
               genericTypeDefinition == typeof(Tuple<,,,,,,,>);
    }

    public static bool IsTypeOf<T>(this object obj)
    {
        return obj.GetType() == typeof(T);
    }

    public static bool IsTypeOrNullableOf<T>(this Type theType)
    {
        var otherType = typeof(T);
        return theType == otherType ||
               (theType.IsNullableOfT() && theType.GetGenericArguments()[0] == otherType);
    }

    public static bool IsValueTuple(Type tuple)
    {
        if (!tuple.IsGenericType)
            return false;
        var openType = tuple.GetGenericTypeDefinition();
        return openType == typeof(ValueTuple<>)
               || openType == typeof(ValueTuple<,>)
               || openType == typeof(ValueTuple<,,>)
               || openType == typeof(ValueTuple<,,,>)
               || openType == typeof(ValueTuple<,,,,>)
               || openType == typeof(ValueTuple<,,,,,>)
               || openType == typeof(ValueTuple<,,,,,,>)
               || (openType == typeof(ValueTuple<,,,,,,,>) && IsTuple(tuple.GetGenericArguments()[7]));
    }

    public static string PrettyPrint(this Type type)
    {
        return type.PrettyPrint(t => t.Name);
    }

    public static string PrettyPrint(this Type type, Func<Type, string> selector)
    {
        var typeName = selector(type);
        var typeInfo = type.GetTypeInfo();
        if (!typeInfo.IsGenericType)
        {
            return typeName;
        }

        var genericParamSelector = typeInfo.IsGenericTypeDefinition ? t => t.Name : selector;
        var genericTypeList = string.Join(",", type.GetGenericArguments().Select(genericParamSelector).ToArray());
        var tickLocation = typeName.IndexOf('`');
        if (tickLocation >= 0)
        {
            typeName = typeName.Substring(0, tickLocation);
        }

        return $"{typeName}<{genericTypeList}>";
    }

    public static bool PropertyMatches(this PropertyInfo prop1, PropertyInfo prop2)
    {
        return prop1.DeclaringType == prop2.DeclaringType && prop1.Name == prop2.Name;
    }

    public static DbType ToDbType(this Type type)
    {
        var typeMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
            // [typeof(Binary)] = DbType.Binary
        };

        return typeMap[type];
    }

    public static T? ToNullable<T>(this T value) where T : struct
    {
        return value.Equals(default(T)) ? null : value;
    }

    private static string GetPrettyNameOf(this Type type, List<Type> travesed)
    {
        travesed.Add(type);

        var prefixName = string.Empty;
        if (type.DeclaringType != null)
        {
            if (!travesed.Contains(type.DeclaringType))
                prefixName = GetPrettyNameOf(type.DeclaringType, travesed) + ".";
        }
        else if (!string.IsNullOrEmpty(type.Namespace))
            prefixName = type.Namespace + ".";

        if (type.IsGenericType)
        {
            var genargNames = type.GetGenericArguments().Select(type1 => GetPrettyNameOf(type1, new List<Type>()));
            var idx = type.Name.IndexOf('`');
            var typename = idx > 0 ? type.Name.Substring(0, idx) : type.Name;
            return $"{prefixName}{typename}<{string.Join(", ", genargNames.ToArray())}>";
        }

        if (type.IsArray)
        {
            return $"{GetPrettyName(type.GetElementType())}[{new string(Enumerable.Repeat(',', type.GetArrayRank() - 1).ToArray())}]";
        }

        return $"{prefixName}{type.Name}";
    }

    private static bool IsOneOfAttributes(Type attribute, object[] objects)
    {
        foreach (var attr in objects)
        {
            var currentAttr = attr.GetType();
            if (currentAttr == attribute)
                return true;
        }

        return false;
    }
}