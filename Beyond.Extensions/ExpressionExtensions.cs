// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using Beyond.Extensions.TypeExtended;

namespace Beyond.Extensions.ExpressionExtended;

public static class ExpressionExtensions
{
    public static Expression<T> AddParameter<T>(this Expression<T> expression, ParameterExpression parameter)
    {
        var parameters = expression.Parameters.Append(parameter).ToList();
        return expression.Update(expression.Body, parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

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

    public static IEnumerable<Expression> AsEnumerableOperands(this BinaryExpression expression)
    {
        return new[] { expression.Left, expression.Right };
    }

    public static Predicate<T> AsPredicate<T>(this Expression<Func<T, bool>> expression)
    {
        var compiled = expression.Compile();
        return new Predicate<T>(compiled);
    }

    public static T BodyAs<T>(this LambdaExpression expression) where T : Expression
    {
        return (T)expression.Body;
    }

    public static Expression CallInstance(this Expression target, string methodName, params Expression[] arguments)
    {
        var method = target.Type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        if (method == null)
        {
            throw new ArgumentException($"No public instance method '{methodName}' defined for type '{target.Type}'.");
        }

        return Expression.Call(target, method, arguments);
    }

    public static Expression CallStatic<T>(this Expression target, string methodName, params Expression[] arguments)
    {
        var method = typeof(T).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        if (method == null)
        {
            throw new ArgumentException($"No public static method '{methodName}' defined for type '{typeof(T)}'.");
        }

        return Expression.Call(null, method, arguments);
    }

    public static Expression<TDelegate> Combine<TDelegate>(this Expression<TDelegate> first, Expression<TDelegate> second, Func<Expression, Expression, BinaryExpression> merge)
    {
        ParameterExpression[] parameters = first.Parameters.Concat(second.Parameters.Skip(1)).ToArray();
        var body = merge(first.Body, new ReplaceExpressionVisitor(second.Parameters[0], first.Parameters[0]).Visit(second.Body) ?? throw new InvalidOperationException());
        return Expression.Lambda<TDelegate>(body, parameters);
    }

    public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // Build parameter map (from parameters of second to parameters of first)
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

        // Replace parameters in the second lambda expression with parameters from the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // Apply the merge function to the body of the two lambda expressions
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    public static bool Contains(this LambdaExpression expression, Expression subexpression)
    {
        var visitor = new ExpressionContainsVisitor(subexpression);
        visitor.Visit(expression);
        return visitor.Contains;
    }

    public static bool ContainsParameter(this LambdaExpression expression, string parameterName)
    {
        return expression.Parameters.Any(p => p.Name == parameterName);
    }

    public static Expression<Func<TTo, TResult>> Convert<TFrom, TTo, TResult>(this Expression<Func<TFrom, TResult>> expression, Func<Expression, Expression> convert)
    {
        var parameter = Expression.Parameter(typeof(TTo), "param");
        var body = new ReplaceExpressionVisitor(expression.Parameters[0], convert(parameter)).Visit(expression.Body);
        if (body != null) return Expression.Lambda<Func<TTo, TResult>>(body, parameter);
        throw new ArgumentNullException("body");
    }

    public static Expression ConvertTypeTo<T>(this Expression expression)
    {
        return Expression.Convert(expression, typeof(T));
    }

    public static Expression ConvertTypeTo(this Expression expression, Type type)
    {
        return Expression.Convert(expression, type);
    }

    public static object? Evaluate(this Expression expr)
    {
        switch (expr.NodeType)
        {
            case ExpressionType.Constant:
                return ((ConstantExpression)expr).Value;

            case ExpressionType.MemberAccess:
                var member = (MemberExpression)expr;
                var obj = member.Expression == null ? null : Evaluate(member.Expression);
                switch (member.Member.MemberType)
                {
                    case MemberTypes.Field:
                        return ((FieldInfo)member.Member).GetValue(obj);

                    case MemberTypes.Property:
                        return ((PropertyInfo)member.Member).GetValue(obj);

                    default:
                        return Expression.Lambda(expr).Compile().DynamicInvoke();
                }
        }
        throw new NotSupportedException(expr.NodeType + " is not supported.");
    }

    public static Expression<Func<T, bool>> False<T>()
    {
        return _ => false;
    }

    public static List<Expression> Flatten(this Expression expression)
    {
        var result = new List<Expression>();
        var stack = new Stack<Expression>();
        stack.Push(expression);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            result.Add(current);

            foreach (var child in GetChildren(current))
            {
                stack.Push(child);
            }
        }

        return result;

        static IEnumerable<Expression> GetChildren(Expression expression)
        {
            foreach (var field in expression.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType.IsSubclassOf(typeof(Expression)))
                {
                    yield return (Expression)field.GetValue(expression);
                }
                else if (field.FieldType.IsAssignableFrom(typeof(IEnumerable<Expression>)))
                {
                    foreach (var child in (IEnumerable<Expression>)field.GetValue(expression))
                    {
                        yield return child;
                    }
                }
            }
        }
    }

    public static bool GetBoolValue(this LambdaExpression expression)
    {
        if (expression.ReturnType != typeof(bool))
        {
            throw new ArgumentException("The lambda expression must return a boolean.");
        }

        var func = expression.Compile();
        return (bool)(func.DynamicInvoke() ?? throw new InvalidOperationException());
    }

    public static MethodInfo? GetCalledMethod(this LambdaExpression expression)
    {
        var methodCallExpression = expression.Body as MethodCallExpression;
        return methodCallExpression?.Method;
    }

    public static IEnumerable<object> GetConstants(this LambdaExpression expression)
    {
        var visitor = new ConstantExpressionVisitor();
        visitor.Visit(expression);
        return visitor.Values;
    }

    public static object? GetConstValue(this Expression expr)
    {
        switch (expr.NodeType)
        {
            case ExpressionType.Constant:
                return ((ConstantExpression)expr).Value;

            case ExpressionType.MemberAccess:
                var member = (MemberExpression)expr;
                var obj = member.Expression == null ? null : Evaluate(member.Expression);
                switch (member.Member.MemberType)
                {
                    case MemberTypes.Field:
                        return ((FieldInfo)member.Member).GetValue(obj);

                    case MemberTypes.Property:
                        return ((PropertyInfo)member.Member).GetValue(obj);

                    default:
                        throw new NotSupportedException(expr.NodeType + " is not supported.");
                }
                break;
        }
        throw new NotSupportedException(expr.NodeType + " is not supported.");
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

    public static IEnumerable<MethodInfo> GetMethods(this LambdaExpression expression)
    {
        var visitor = new MethodCallVisitor();
        visitor.Visit(expression);
        return visitor.Methods;
    }

    public static IEnumerable<Type> GetParameterTypes(this LambdaExpression expression)
    {
        return expression.Parameters.Select(p => p.Type);
    }

    public static string GetPropertyName<TSource, TProperty>(this Expression<Func<TSource, TProperty>> property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        return property.GetMemberExpression().Member.Name;
    }

    public static string GetPropertyPath(this Expression expression)
    {
        var path = new List<string>();
        var node = expression;

        while (node != null)
        {
            switch (node)
            {
                case MemberExpression memberExpression:
                    path.Add(memberExpression.Member.Name);
                    node = memberExpression.Expression;
                    break;

                case MethodCallExpression { Method.Name: "get_Item" } methodCallExpression:
                    path.Add($"[{methodCallExpression.Arguments[0].ToReadableString()}]");
                    node = methodCallExpression.Object;
                    break;

                case MethodCallExpression methodCallExpression:
                    path.Add($"{methodCallExpression.Method.Name}({string.Join(", ", methodCallExpression.Arguments.Select(arg => arg.ToReadableString()))})");
                    node = methodCallExpression.Object;
                    break;

                case LambdaExpression lambdaExpression:
                    node = lambdaExpression.Body;
                    break;

                default:
                    node = null;
                    break;
            }
        }

        path.Reverse();
        return string.Join(".", path);
    }

    public static object? GetValue(this Expression expression)
    {
        switch (expression)
        {
            case null:
                throw new ArgumentNullException(nameof(expression));
            // If the expression represents a lambda expression, compile it and invoke it
            case LambdaExpression lambda:
                {
                    var fn = lambda.Compile();
                    return fn.DynamicInvoke(null);
                }
            // If the expression is a constant, return the constant value
            case ConstantExpression constant:
                return constant.Value;

            default:
                // Otherwise, interpret the expression
                switch (expression.NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                    case ExpressionType.Power:
                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.LeftShift:
                    case ExpressionType.RightShift:
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.Coalesce:
                    case ExpressionType.ArrayIndex:
                    case ExpressionType.RightShiftAssign:
                    case ExpressionType.LeftShiftAssign:
                    case ExpressionType.AddAssign:
                    case ExpressionType.SubtractAssign:
                    case ExpressionType.MultiplyAssign:
                    case ExpressionType.AndAssign:
                    case ExpressionType.OrAssign:
                    case ExpressionType.PowerAssign:
                    case ExpressionType.Assign:
                        var binary = (BinaryExpression)expression;
                        return Expression.Lambda(binary).Compile().DynamicInvoke();

                    default:
                        throw new NotSupportedException($"The operation '{expression.NodeType}' is not supported");
                }
        }
    }

    public static bool HasAttribute<TAttribute>(this LambdaExpression expression) where TAttribute : Attribute
    {
        if (expression.Body.NodeType == ExpressionType.MemberAccess)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;
            var attributes = propertyInfo.GetCustomAttributes(typeof(TAttribute), false);
            return attributes.Length > 0;
        }

        return false;
    }

    public static bool HasParameter<T>(this Expression<Func<T, bool>> expression, string paramName)
    {
        var visitor = new HasParameterVisitor(paramName);
        visitor.Visit(expression);
        return visitor.HasParameter;
    }

    public static Expression<Func<T, bool>> IfThen<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(expr1.Body, new ReplaceExpressionVisitor(expr2.Parameters.Single(), expr1.Parameters.Single()).Visit(expr2.Body) ?? throw new InvalidOperationException()),
            expr1.Parameters);
    }

    public static Expression<Func<T, bool>> Invert<T>(this Expression<Func<T, bool>> expression)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
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

    public static TResult Invoke<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T1 t1,
        T2 t2, T3 t3)
    {
        return expression.Compile()(t1, t2, t3);
    }

    public static TResult Invoke<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression,
        T1 t1, T2 t2, T3 t3, T4 t4)
    {
        return expression.Compile()(t1, t2, t3, t4);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
    {
        return expression.Compile()(t1, t2, t3, t4, t5);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6,
        T7 t7)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5,
        T6 t6, T7 t7, T8 t8)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4,
        T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4,
        T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T1 t1, T2 t2, T3 t3,
        T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T1 t1, T2 t2,
        T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T1 t1, T2 t2,
        T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T1 t1,
        T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression,
        T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14,
        T15 t15)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
    }

    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>>
            expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12,
        T13 t13, T14 t14, T15 t15, T16 t16)
    {
        return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
    }

    public static Expression<Func<T, bool>> Invoke<T>(this Expression<Func<T, bool>> expr, T value)
    {
        var parameter = Expression.Parameter(typeof(T), "param");
        var visitor = new ReplaceExpressionVisitor(expr.Parameters[0], Expression.Constant(value, typeof(T)));
        var body = visitor.Visit(expr.Body) ?? throw new ArgumentNullException("visitor.Visit(expr.Body)");

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    public static bool IsBinary(this LambdaExpression expression)
    {
        return expression.Body is BinaryExpression;
    }

    public static bool IsBinary(this Expression expression)
    {
        return expression.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse;
    }

    public static bool IsBoolean(this Expression expression)
    {
        return expression.Type == typeof(bool);
    }

    public static bool IsConstant(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Constant;
    }

    public static bool IsDateTime(this Expression expression)
    {
        return expression.Type == typeof(DateTime);
    }

    public static bool IsDateTimeOffset(this Expression expression)
    {
        return expression.Type == typeof(DateTimeOffset);
    }

    public static bool IsDecimal(this Expression expression)
    {
        return expression.Type == typeof(double) || expression.Type == typeof(decimal);
    }

    public static bool IsEnumerable(this Expression expression)
    {
        return expression.Type.IsEnumerable();
    }

    public static bool IsFalse(this Expression expression)
    {
        var value = ((ConstantExpression)expression).Value;
        return value != null && expression.NodeType == ExpressionType.Constant && value.Equals(false);
    }

    public static bool IsInteger(this Expression expression)
    {
        return expression.Type == typeof(int) || expression.Type == typeof(long);
    }

    public static bool IsLambda(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Lambda;
    }

    public static bool IsMemberAccess(this Expression expression)
    {
        return expression.NodeType == ExpressionType.MemberAccess;
    }

    public static bool IsMethodCall(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Call;
    }

    public static Expression<Func<T, bool>> IsNotNull<T, TProperty>(this Expression<Func<T, TProperty>> property)
    {
        var propertyParameter = property.Parameters.Single();
        var notNullCheck = Expression.NotEqual(property.Body, Expression.Constant(null, typeof(TProperty)));

        return Expression.Lambda<Func<T, bool>>(notNullCheck, propertyParameter);
    }

    public static bool IsNotNull(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value != null;
    }

    public static Expression<Func<T, bool>> IsNull<T, TProperty>(this Expression<Func<T, TProperty>> property)
    {
        var propertyParameter = property.Parameters.Single();
        var nullCheck = Expression.Equal(property.Body, Expression.Constant(null, typeof(TProperty)));

        return Expression.Lambda<Func<T, bool>>(nullCheck, propertyParameter);
    }

    public static bool IsNull(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null;
    }

    public static bool IsNumeric(this Expression expression)
    {
        return expression.Type == typeof(int) || expression.Type == typeof(long) || expression.Type == typeof(double) || expression.Type == typeof(decimal);
    }

    public static bool IsParameter(this Expression expression)
    {
        return expression.NodeType == ExpressionType.Parameter;
    }

    public static bool IsQueryable(this Expression expression)
    {
        return expression.Type.IsQueryable();
    }

    public static bool IsString(this Expression expression)
    {
        return expression.Type == typeof(string);
    }

    public static bool IsTrue(this Expression expression)
    {
        var value = ((ConstantExpression)expression).Value;
        return value != null && expression.NodeType == ExpressionType.Constant && value.Equals(true);
    }

    public static bool IsUnary(this Expression expression)
    {
        return expression.NodeType is ExpressionType.Not;
    }

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
        if (expr == null) throw new ArgumentNullException(nameof(expr));
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters[0]);
    }

    public static Expression<TFunc> Not<TFunc>(this Expression<TFunc> expression)
    {
        var param = expression.Parameters;
        var body = Expression.Not(expression.Body);
        return Expression.Lambda<TFunc>(body, param);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
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

    public static Expression PartialEval(this Expression expression, Func<Expression, bool> canBeEvaluated)
    {
        return new PartialEvalVisitor(canBeEvaluated).Visit(expression);
    }

    public static Expression RemoveCast(this UnaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.Convert)
        {
            return expression.Operand;
        }

        return expression;
    }

    public static Expression<T> RemoveParameter<T>(this Expression<T> expression, ParameterExpression parameter)
    {
        var parameters = expression.Parameters.Where(p => p != parameter).ToList();
        return expression.Update(expression.Body, parameters);
    }

    public static Expression? Replace(this Expression expression, Expression searchEx, Expression replaceEx)
    {
        return new ReplaceExpressionVisitor(searchEx, replaceEx).Visit(expression);
    }

    public static BinaryExpression SwapLeftRight(this BinaryExpression binary)
    {
        return Expression.MakeBinary(binary.NodeType, binary.Right, binary.Left);
    }

    public static Expression<Func<T, TU>> ToExpressionOfFunc<T, TU>(Expression<Action<T>> expr)
    {
        return Expression.Lambda<Func<T, TU>>(expr.Body, expr.Parameters);
    }

    public static PropertyInfo ToPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body.NodeType != ExpressionType.MemberAccess)
        {
            throw new ArgumentException("The expression must access a member.", nameof(expression));
        }

        return (PropertyInfo)((MemberExpression)expression.Body).Member;
    }

    public static string? ToReadableString(this Expression expression)
    {
        switch (expression)
        {
            case ConstantExpression constant:
                return constant.Value?.ToString();

            case MemberExpression member:
                return member.Member.Name;

            case MethodCallExpression methodCall:
                return $"{methodCall.Method.Name}({string.Join(", ", methodCall.Arguments.Select(arg => arg.ToReadableString()))})";

            case BinaryExpression binary:
                return $"{binary.Left.ToReadableString()} {binary.NodeType} {binary.Right.ToReadableString()}";

            case UnaryExpression unary:
                return $"{unary.NodeType} {unary.Operand.ToReadableString()}";

            case ParameterExpression parameter:
                return parameter.Name;

            case NewExpression newExpr:
                return $"new {newExpr.Type.Name}({string.Join(", ", newExpr.Arguments.Select(arg => arg.ToReadableString()))})";

            case ConditionalExpression conditional:
                return $"{conditional.Test.ToReadableString()} ? {conditional.IfTrue.ToReadableString()} : {conditional.IfFalse.ToReadableString()}";

            case IndexExpression index:
                return $"{index.Object?.ToReadableString()}[{string.Join(", ", index.Arguments.Select(arg => arg.ToReadableString()))}]";

            case InvocationExpression invocation:
                return $"{invocation.Expression.ToReadableString()}({string.Join(", ", invocation.Arguments.Select(arg => arg.ToReadableString()))})";

            case LambdaExpression lambda:
                return $"{string.Join(", ", lambda.Parameters.Select(p => p.ToReadableString()))} => {lambda.Body.ToReadableString()}";

            case NewArrayExpression newArray:
                return $"new {newArray.Type.GetElementType()?.Name}[{newArray.Expressions.Count}]";

            case TypeBinaryExpression typeBinary:
                return $"{typeBinary.Expression.ToReadableString()} is {typeBinary.TypeOperand.Name}";

            default:
                throw new NotSupportedException($"Expressions of type '{expression.GetType()}' are not supported.");
        }
    }

    public static Expression Transform(this Expression expression, Func<Expression, Expression> transformFunc)
    {
        return new ExpressionTransformVisitor(transformFunc).Visit(expression);
    }

    public static Expression<Func<T, bool>> True<T>()
    {
        return _ => true;
    }

    public static Expression<T> UpdateBody<T>(this Expression<T> expression, Expression body)
    {
        return expression.Update(body, expression.Parameters);
    }

    public static Expression<Func<TIn, TResult>> WithParameter<TIn, TResult, TOut>(
        this Expression<Func<TIn, TResult>> expression,
        Expression<Func<TIn, TOut>> parameter)
    {
        return Expression.Lambda<Func<TIn, TResult>>(
            new ReplaceExpressionVisitor(expression.Parameters.Single(), parameter.Body).Visit(expression.Body) ?? throw new InvalidOperationException(),
            parameter.Parameters.Single());
    }

    public static Expression<T>? Wrap<T>(this Expression inner, Expression outer)
    {
        var replacer = new ReplaceExpressionVisitor(inner, outer);
        return (Expression<T>?)replacer.Visit(inner);
    }

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

    public class ExpressionContainsVisitor : ExpressionVisitor
    {
        private readonly Expression _subexpression;

        public ExpressionContainsVisitor(Expression subexpression)
        {
            _subexpression = subexpression;
        }

        public bool Contains { get; private set; }

        public override Expression Visit(Expression node)
        {
            if (!Contains && node != null && node.Equals(_subexpression))
            {
                Contains = true;
            }

            return base.Visit(node);
        }
    }

    private class ConstantExpressionVisitor : ExpressionVisitor
    {
        public List<object> Values { get; } = new List<object>();

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null) Values.Add(node.Value);
            return base.VisitConstant(node);
        }
    }

    private class ExpressionTransformVisitor : ExpressionVisitor
    {
        private readonly Func<Expression?, Expression?> _transformFunc;

        public ExpressionTransformVisitor(Func<Expression?, Expression?> transformFunc)
        {
            _transformFunc = transformFunc;
        }

        public override Expression? Visit(Expression? node)
        {
            return base.Visit(_transformFunc(node));
        }
    }

    private class HasParameterVisitor : ExpressionVisitor
    {
        private readonly string _paramName;

        public HasParameterVisitor(string paramName)
        {
            _paramName = paramName;
        }

        public bool HasParameter { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Name == _paramName)
            {
                HasParameter = true;
            }

            return base.VisitParameter(node);
        }
    }

    private class MethodCallVisitor : ExpressionVisitor
    {
        public List<MethodInfo> Methods { get; } = new List<MethodInfo>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Methods.Add(node.Method);
            return base.VisitMethodCall(node);
        }
    }

    private class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression>? map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression>? map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }

    private class PartialEvalVisitor : ExpressionVisitor
    {
        private readonly Func<Expression, bool> canBeEvaluated;

        public PartialEvalVisitor(Func<Expression, bool> canBeEvaluated)
        {
            this.canBeEvaluated = canBeEvaluated;
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                return null;
            }

            if (node.NodeType == ExpressionType.Constant)
            {
                return node;
            }

            if (canBeEvaluated(node))
            {
                return Expression.Constant(Expression.Lambda(node).Compile().DynamicInvoke(), node.Type);
            }

            return base.Visit(node);
        }
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