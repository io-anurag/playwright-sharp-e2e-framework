using AutomationFramework.Core.Constants;
using Serilog;
using Serilog.Events;
using Serilog.Enrichers;

namespace AutomationFramework.Core.Logging;

/// <summary>
/// Singleton Serilog logger service.
/// Writes to both the console (coloured) and a daily rolling log file.
/// Wraps Serilog behind an interface so that callers never take a direct
/// dependency on the Serilog package — only on this service.
/// </summary>
public sealed class LoggerService
{
    private static readonly Lazy<LoggerService> _instance =
        new(() => new LoggerService(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static LoggerService Instance => _instance.Value;

    private readonly ILogger _logger;

    private LoggerService()
    {
        var logsDir = ResolveLogsDirectory();
        Directory.CreateDirectory(logsDir);

        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.File(
                path: Path.Combine(logsDir, ".log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [T{ThreadId}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30,
                shared: true)
            .Enrich.WithThreadId()
            .CreateLogger();
    }

    // ── Logging methods ───────────────────────────────────────────────────────

    public void Information(string messageTemplate, params object?[] args) =>
        _logger.Information(messageTemplate, args);

    public void Debug(string messageTemplate, params object?[] args) =>
        _logger.Debug(messageTemplate, args);

    public void Warning(string messageTemplate, params object?[] args) =>
        _logger.Warning(messageTemplate, args);

    public void Error(string messageTemplate, params object?[] args) =>
        _logger.Error(messageTemplate, args);

    public void Error(Exception ex, string messageTemplate, params object?[] args) =>
        _logger.Error(ex, messageTemplate, args);

    public void Fatal(string messageTemplate, params object?[] args) =>
        _logger.Fatal(messageTemplate, args);

    public void Fatal(Exception ex, string messageTemplate, params object?[] args) =>
        _logger.Fatal(ex, messageTemplate, args);

    // ── Private helpers ───────────────────────────────────────────────────────

    private static string ResolveLogsDirectory()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return Path.Combine(dir.FullName, TestConstants.LogsDirectory);
            dir = dir.Parent;
        }
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.LogsDirectory);
    }
}
