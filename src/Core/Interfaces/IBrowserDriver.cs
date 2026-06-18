using Microsoft.Playwright;

namespace AutomationFramework.Core.Interfaces;

/// <summary>
/// Abstraction over a running browser instance.
/// Enables the Factory Pattern — concrete implementations can wrap
/// Playwright, Selenium, or any other driver without changing test code.
/// </summary>
public interface IBrowserDriver : IAsyncDisposable
{
    /// <summary>The active Playwright page (tab).</summary>
    IPage Page { get; }

    /// <summary>The underlying browser instance.</summary>
    IBrowser Browser { get; }

    /// <summary>The browser context (enables cookie/session isolation per test).</summary>
    IBrowserContext Context { get; }

    /// <summary>Initialises the browser and opens a new context + page.</summary>
    Task InitializeAsync();

    /// <summary>Captures a full-page screenshot and returns the raw bytes.</summary>
    Task<byte[]> TakeScreenshotAsync();

    /// <summary>Saves a screenshot to disk and returns the file path.</summary>
    Task<string> SaveScreenshotAsync(string directory, string fileName);
}
