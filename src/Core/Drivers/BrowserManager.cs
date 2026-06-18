using AutomationFramework.Core.Configuration;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Core.Drivers;

/// <summary>
/// Manages the lifecycle of browser driver instances scoped to the current
/// asynchronous execution context (test).
///
/// Uses <see cref="AsyncLocal{T}"/> so that each parallel NUnit test worker
/// gets its own browser without sharing state — critical for parallel safety.
/// </summary>
public sealed class BrowserManager : IDisposable
{
    private static readonly AsyncLocal<IBrowserDriver?> _currentDriver = new();

    private readonly AppConfigurationManager _config;
    private bool _disposed;

    public BrowserManager(AppConfigurationManager config)
    {
        _config = config;
    }

    /// <summary>Gets the browser driver bound to the current async context.</summary>
    public static IBrowserDriver? Current => _currentDriver.Value;

    /// <summary>
    /// Creates, initialises, and registers a new browser driver for the
    /// current async context. Safe to call concurrently from different tests.
    /// </summary>
    public async Task<IBrowserDriver> StartBrowserAsync()
    {
        var driver = await BrowserFactory.CreateAsync(
            _config.GetBrowser(),
            _config.IsHeadless());

        await driver.InitializeAsync();
        _currentDriver.Value = driver;

        LoggerService.Instance.Information(
            "Browser started | Browser: {Browser} | Headless: {Headless}",
            _config.GetBrowser(), _config.IsHeadless());

        return driver;
    }

    /// <summary>
    /// Disposes the browser bound to the current async context and clears the slot.
    /// </summary>
    public async Task StopBrowserAsync()
    {
        var driver = _currentDriver.Value;
        if (driver is not null)
        {
            await driver.DisposeAsync();
            _currentDriver.Value = null;
            LoggerService.Instance.Information("Browser stopped.");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        // Synchronous dispose — best effort
        _currentDriver.Value?.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
