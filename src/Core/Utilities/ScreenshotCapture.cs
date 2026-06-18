using AutomationFramework.Core.Constants;
using AutomationFramework.Core.Helpers;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Core.Utilities;

/// <summary>
/// Handles screenshot capture and persistence.
/// Decouples screenshot logic from both page objects and test base classes.
/// </summary>
public sealed class ScreenshotCapture
{
    private readonly string _screenshotsRoot;

    public ScreenshotCapture(string? screenshotsRoot = null)
    {
        _screenshotsRoot = screenshotsRoot ?? ResolveScreenshotsRoot();
    }

    /// <summary>
    /// Captures a full-page screenshot and saves it to
    /// screenshots/{testName}_{timestamp}.png.
    /// Returns the absolute path of the saved file.
    /// </summary>
    public async Task<string> CaptureAsync(IBrowserDriver driver, string testName)
    {
        var safeTestName = SanitizeFileName(testName);
        var fileName     = $"{safeTestName}_{DateTimeHelper.FileSafeTimestamp()}.png";
        var directory    = Path.Combine(_screenshotsRoot, DateTimeHelper.TodayFormatted());

        var path = await driver.SaveScreenshotAsync(directory, fileName);
        LoggerService.Instance.Information("Screenshot captured: {Path}", path);
        return path;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
    }

    private static string ResolveScreenshotsRoot()
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
