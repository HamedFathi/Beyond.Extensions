// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Beyond.Extensions.ExpressionExtended;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        if (expr1 == null) throw new ArgumentNullException(nameof(expr1));
        if (expr2 == null) throw new ArgumentNullException(nameof(expr2));

        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
    }

    public static T Evaluate<T>(this Expression<Func<T>> expression) => expression.Compile()();

    public static MemberExpression GetMemberExpression<TSource, TProperty>(
        this Expression<Func<TSource, TProperty>> property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (Equals(property, null)) throw new NullReferenceException($"{nameof(property)} is required.");

        var expr = property.Body switch
        {
            MemberExpression body => body,
            UnaryExpression expression => (MemberExpression)expression.Operand,
            _ => throw new ArgumentException($"Expression '{property}' is not supported.", nameof(property))
        };

        return expr;
    }

    public static string GetPropertyName<TSource, TProperty>(this Expression<Func<TSource, TProperty>> property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        return property.GetMemberExpression().Member.Name;
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        if (expr == null) throw new ArgumentNullException(nameof(expr));
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters[0]);
    }

    public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        if (expr1 == null) throw new ArgumentNullException(nameof(expr1));
        if (expr2 == null) throw new ArgumentNullException(nameof(expr2));

        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left!, right!), parameter);
    }

    public static Expression<Func<T, U>> ToExpressionOfFunc<T, U>(Expression<Action<T>> expr)
    {
        return Expression.Lambda<Func<T, U>>(expr.Body, expr.Parameters);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression? _newValue;
        private readonly Expression _oldValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression? newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return ReferenceEquals(node, _oldValue) ? _newValue : base.Visit(node);
        }
    }
}