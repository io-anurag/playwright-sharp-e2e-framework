namespace AutomationFramework.Core.Helpers;

/// <summary>
/// Date/time helpers used for generating unique file names, log timestamps,
/// and test data values.
/// </summary>
public static class DateTimeHelper
{
    /// <summary>Returns a file-safe timestamp string: yyyyMMdd_HHmmss.</summary>
    public static string FileSafeTimestamp() =>
        DateTime.Now.ToString("yyyyMMdd_HHmmss");

    /// <summary>Returns a log-friendly timestamp: yyyy-MM-dd HH:mm:ss.</summary>
    public static string LogTimestamp() =>
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    /// <summary>Returns today's date formatted as yyyyMMdd.</summary>
    public static string TodayFormatted() =>
        DateTime.Now.ToString("yyyyMMdd");

    /// <summary>Returns a unique identifier suitable for test data names.</summary>
    public static string UniqueId() =>
        $"{DateTime.Now:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";

    /// <summary>Converts Unix epoch milliseconds to a <see cref="DateTime"/>.</summary>
    public static DateTime FromUnixMs(long milliseconds) =>
        DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).LocalDateTime;

    /// <summary>Returns the elapsed time description, e.g. "2m 15s".</summary>
    public static string Elapsed(DateTime start)
    {
        var elapsed = DateTime.Now - start;
        return elapsed.TotalMinutes >= 1
            ? $"{(int)elapsed.TotalMinutes}m {elapsed.Seconds}s"
            : $"{elapsed.TotalSeconds:F1}s";
    }
}
