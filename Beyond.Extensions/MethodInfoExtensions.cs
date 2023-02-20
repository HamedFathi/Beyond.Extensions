// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Beyond.Extensions.MethodInfoExtended;

public static class MethodInfoExtensions
{
    public static Delegate? CreateDelegateWithTarget(this MethodInfo? method, object? target)
    {
        if (method is null || target is null)
        {
            return null;
        }

        if (method.IsStatic)
            return null;

        if (method.IsGenericMethod)
            return null;

        return method.CreateDelegate(Expression.GetDelegateType(
            (from parameter in method.GetParameters() select parameter.ParameterType)
            .Concat(new[] { method.ReturnType })
            .ToArray()), target);
    }
}