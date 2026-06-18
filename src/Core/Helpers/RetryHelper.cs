using AutomationFramework.Core.Logging;

namespace AutomationFramework.Core.Helpers;

/// <summary>
/// Provides a configurable retry mechanism for flaky operations.
/// Implements the Retry Pattern — callers pass an async lambda and let
/// the helper manage delays and exception handling.
/// </summary>
public static class RetryHelper
{
    /// <summary>
    /// Retries <paramref name="action"/> up to <paramref name="maxAttempts"/> times,
    /// waiting <paramref name="delayMs"/> between each attempt.
    /// Returns on the first success; rethrows the last exception on exhaustion.
    /// </summary>
    public static async Task RetryAsync(
        Func<Task> action,
        int maxAttempts = 3,
        int delayMs     = 1_000,
        string operationName = "operation")
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                LoggerService.Instance.Warning(
                    "Retry {Attempt}/{Max} for '{Operation}' failed: {Message}",
                    attempt, maxAttempts, operationName, ex.Message);

                if (attempt < maxAttempts)
                    await Task.Delay(delayMs);
            }
        }

        throw lastException!;
    }

    /// <summary>
    /// Retries <paramref name="func"/> and returns its result on the first success.
    /// </summary>
    public static async Task<T> RetryAsync<T>(
        Func<Task<T>> func,
        int maxAttempts = 3,
        int delayMs     = 1_000,
        string operationName = "operation")
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                lastException = ex;
                LoggerService.Instance.Warning(
                    "Retry {Attempt}/{Max} for '{Operation}' failed: {Message}",
                    attempt, maxAttempts, operationName, ex.Message);

                if (attempt < maxAttempts)
                    await Task.Delay(delayMs);
            }
        }

        throw lastException!;
    }

    /// <summary>
    /// Polls <paramref name="condition"/> every <paramref name="intervalMs"/> until it returns
    /// true or <paramref name="timeoutMs"/> elapses.
    /// </summary>
    public static async Task<bool> WaitUntilAsync(
        Func<Task<bool>> condition,
        int timeoutMs  = 10_000,
        int intervalMs = 500)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            if (await condition())
                return true;
            await Task.Delay(intervalMs);
        }
        return false;
    }
}
