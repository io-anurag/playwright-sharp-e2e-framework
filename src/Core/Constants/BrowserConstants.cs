namespace AutomationFramework.Core.Constants;

/// <summary>
/// String constants for browser identifiers.
/// Use these instead of magic strings throughout the framework.
/// </summary>
public static class BrowserConstants
{
    public const string Chromium = "chromium";
    public const string Chrome   = "chrome";
    public const string Firefox  = "firefox";
    public const string Edge     = "edge";
    public const string WebKit   = "webkit";

    public static readonly IReadOnlyList<string> All =
        [Chromium, Chrome, Firefox, Edge, WebKit];
}
