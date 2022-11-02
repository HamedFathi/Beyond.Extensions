// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Beyond.Extensions.ExceptionExtended;

public static class ExceptionExtensions
{
    public static string ToFormattedString(this Exception exception)
    {
        var messages = exception
            .GetAllExceptions()
            .Where(e => !string.IsNullOrWhiteSpace(e.Message))
            .Select(e => e.Message.Trim());
        return string.Join(Environment.NewLine, messages);
    }

    private static IEnumerable<Exception> GetAllExceptions(this Exception exception)
    {
        yield return exception;

        if (exception is AggregateException aggrEx)
            foreach (var innerEx in aggrEx.InnerExceptions.SelectMany(e => e.GetAllExceptions()))
                yield return innerEx;
        else if (exception.InnerException != null)
            foreach (var innerEx in exception.InnerException.GetAllExceptions())
                yield return innerEx;
    }
}