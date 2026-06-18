using AutomationFramework.Core.Constants;
using AutomationFramework.Core.Helpers;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Infrastructure.Screenshots;

/// <summary>
/// Manages screenshot capture, naming, and storage.
/// Decouples screenshot concerns from test base classes and reporters.
/// </summary>
public sealed class ScreenshotManager
{
    private readonly string _rootDirectory;

    public ScreenshotManager(string? rootDirectory = null)
    {
        _rootDirectory = rootDirectory ?? FindScreenshotsRoot();
    }

    /// <summary>
    /// Captures and saves a screenshot for the current test.
    /// Returns the absolute path of the saved PNG file, or null on failure.
    /// </summary>
    public async Task<string?> CaptureOnFailureAsync(IBrowserDriver driver, string testName)
    {
        try
        {
            var dir      = Path.Combine(_rootDirectory, DateTimeHelper.TodayFormatted());
            var fileName = $"{SanitizeName(testName)}_{DateTimeHelper.FileSafeTimestamp()}.png";
            var path     = await driver.SaveScreenshotAsync(dir, fileName);

            LoggerService.Instance.Information("Failure screenshot: {Path}", path);
            return path;
        }
        catch (Exception ex)
        {
            LoggerService.Instance.Error(ex, "Failed to capture screenshot for test: {TestName}", testName);
            return null;
        }
    }

    /// <summary>
    /// Captures a named screenshot (e.g., for a specific step).
    /// </summary>
    public async Task<string?> CaptureStepAsync(IBrowserDriver driver, string stepName)
    {
        try
        {
            var dir      = Path.Combine(_rootDirectory, "steps", DateTimeHelper.TodayFormatted());
            var fileName = $"{SanitizeName(stepName)}_{DateTimeHelper.FileSafeTimestamp()}.png";
            return await driver.SaveScreenshotAsync(dir, fileName);
        }
        catch (Exception ex)
        {
            LoggerService.Instance.Error(ex, "Failed to capture step screenshot: {StepName}", stepName);
            return null;
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static string SanitizeName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray())
            .Replace(' ', '_')
            .Truncate(80);
    }

    private static string FindScreenshotsRoot()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return Path.Combine(dir.FullName, TestConstants.ScreenshotsDirectory);
            dir = dir.Parent;
        }
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.ScreenshotsDirectory);
    }
}

file static class StringExt
{
    public static string Truncate(this string s, int max) =>
        s.Length <= max ? s : s[..max];
}
