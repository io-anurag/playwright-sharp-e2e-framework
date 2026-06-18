namespace AutomationFramework.Core.Enums;

/// <summary>
/// Supported browser types.
/// Maps to Playwright launch options and the BROWSER environment variable.
/// </summary>
public enum BrowserType
{
    Chromium,
    Chrome,
    Firefox,
    Edge,
    WebKit
}
