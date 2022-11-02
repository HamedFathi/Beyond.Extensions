﻿// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.ValueTaskExtended;

public static class ValueTaskExtensions
{
    public static async void SafeFireAndForget(this ValueTask @this, bool continueOnCapturedContext = true,
        Action<Exception>? onException = null)
    {
        try
        {
            await @this.ConfigureAwait(continueOnCapturedContext);
        }
        catch (Exception e) when (onException != null)
        {
            onException(e);
        }
    }
}