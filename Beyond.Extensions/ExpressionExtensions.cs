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
    public static Expression ConvertTypeTo<T>(this Expression expression)
    {
        return Expression.Convert(expression, typeof(T));
    }

    public static Expression ConvertTypeTo(this Expression expression, Type type)
    {
        return Expression.Convert(expression, type);
    }
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
    public static MethodInfo GetMethodInfo(this Expression expression)
    {
        var methodCallExpr = (MethodCallExpression)expression;
        return methodCallExpr.Method;
    }
    public static MethodInfo GetMethodInfo<T>(this Expression<Action<T>> expression)
    {
        var methodCallExpr = (MethodCallExpression)expression.Body;
        return methodCallExpr.Method;
    }
    public static string GetPropertyName<TSource, TProperty>(this Expression<Func<TSource, TProperty>> property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        return property.GetMemberExpression().Member.Name;
    }

    public static TResult Invoke<TResult>(this Expression<Func<TResult>> expression) => expression.Compile()();

    public static TResult Invoke<T, TResult>(this Expression<Func<T, TResult>> expression, T t)
    {
        return expression.Compile()(t);
    }

    public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 t1, T2 t2)
    {
        return expression.Compile()(t1, t2);
    }

    public static TResult Invoke<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T1 t1, T2 t2, T3 t3)
    {
        return expression.Compile()(t1, t2, t3);
    }

    public static TResult Invoke<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4)
    {
        return expression.Compile()(t1, t2, t3, t4);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
    {
        return expression.Compile()(t1, t2, t3, t4, t5);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        if (expr == null) throw new ArgumentNullException(nameof(expr));
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters[0]);
    }

    public static Expression<TFunc> Not<TFunc>(this Expression<TFunc> expression)
    {
        var param = expression.Parameters;
        var body = Expression.Not(expression.Body);
        var newExpr = Expression.Lambda<TFunc>(body, param);
        return newExpr;
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
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
           Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> False<T>() { return _ => false; }

    public static Expression<Func<T, bool>> NAnd<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var andExpr = expr1.And(expr2);
        return andExpr.Not();
    }

    public static Expression<Func<T, bool>> Nor<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var orExpr = expr1.Or(expr2);
        return orExpr.Not();
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        var notExpr = Expression.Not(expr.Body);
        return Expression.Lambda<Func<T, bool>>(notExpr, expr.Parameters);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> True<T>() { return _ => true; }
    public static Expression<Func<T, bool>> XNor<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var xorExpr = expr1.Xor(expr2);
        return xorExpr.Not();
    }

    public static Expression<Func<T, bool>> Xor<T>(this Expression<Func<T, bool>> expr1,
                Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.ExclusiveOr(expr1.Body, invokedExpr), expr1.Parameters);
    }
    public static Expression<Func<T, TU>> ToExpressionOfFunc<T, TU>(Expression<Action<T>> expr)
    {
        return Expression.Lambda<Func<T, TU>>(expr.Body, expr.Parameters);
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