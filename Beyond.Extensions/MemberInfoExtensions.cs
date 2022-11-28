// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

using Beyond.Extensions.FieldInfoExtended;
using Beyond.Extensions.PropertyInfoExtended;

namespace Beyond.Extensions.MemberInfoExtended;

public static class MemberInfoExtensions
{
    public static Func<TTarget, TMember> Getter<TTarget, TMember>(this MemberInfo member)
    {
        return member is PropertyInfo info
            ? info.GetProperty<TTarget, TMember>()
            : ((FieldInfo)member).GetField<TTarget, TMember>();
    }

    public static Action<TTarget, TMember>? Setter<TTarget, TMember>(this MemberInfo member)
    {
        return member is PropertyInfo info
            ? info.SetProperty<TTarget, TMember>()
            : ((FieldInfo)member).SetField<TTarget, TMember>();
    }
}