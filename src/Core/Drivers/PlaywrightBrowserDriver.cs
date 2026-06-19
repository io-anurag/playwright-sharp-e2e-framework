using System.Runtime.InteropServices;
using AutomationFramework.Core.Constants;
using AutomationFramework.Core.Enums;
using AutomationFramework.Core.Exceptions;
using AutomationFramework.Core.Logging;
using Microsoft.Playwright;
using PlaywrightBrowserType = Microsoft.Playwright.BrowserType;

namespace AutomationFramework.Core.Drivers;

/// <summary>
/// Concrete implementation of <see cref="Interfaces.IBrowserDriver"/> backed by Playwright.
/// Owns the browser context so that each test gets a completely isolated session
/// (cookies, local storage, authentication state are never shared).
/// </summary>
public sealed class PlaywrightBrowserDriver : Interfaces.IBrowserDriver
{
    private readonly IPlaywright _playwright;
    private readonly bool        _headless;
    private IBrowser?        _browser;
    private IBrowserContext? _context;
    private IPage?           _page;

    public IPage           Page    => _page    ?? throw new DriverException("Page not initialised. Call InitializeAsync() first.");
    public IBrowser        Browser => _browser ?? throw new DriverException("Browser not initialised. Call InitializeAsync() first.");
    public IBrowserContext Context => _context ?? throw new DriverException("Context not initialised. Call InitializeAsync() first.");

    internal PlaywrightBrowserDriver(IPlaywright playwright, IBrowser browser, bool headless = true)
    {
        _playwright = playwright;
        _browser    = browser;
        _headless   = headless;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        // Headed  : ViewportSize.NoViewport + --start-maximized = real window fills the screen.
        // Headless : no OS window exists, so use the primary screen's resolution so screenshots
        //            and layout match what a user would see on this machine.
        _context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize      = _headless ? GetPrimaryScreenViewport() : ViewportSize.NoViewport,
            IgnoreHTTPSErrors = true
        });

        _context.SetDefaultTimeout(TestConstants.DefaultTimeout);
        _context.SetDefaultNavigationTimeout(TestConstants.NavigationTimeout);

        _page = await _context.NewPageAsync();
        LoggerService.Instance.Information("Browser page initialised successfully.");
    }

    /// <inheritdoc />
    public async Task<byte[]> TakeScreenshotAsync()
    {
        return await Page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
    }

    /// <inheritdoc />
    public async Task<string> SaveScreenshotAsync(string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, fileName);
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path     = path,
            FullPage = true
        });
        LoggerService.Instance.Information("Screenshot saved: {Path}", path);
        return path;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_page is not null)    await _page.CloseAsync();
        if (_context is not null) await _context.CloseAsync();
        if (_browser is not null) await _browser.CloseAsync();
        _playwright.Dispose();
    }

    /// <summary>
    /// Returns the primary monitor's resolution for use as the headless viewport.
    /// On Windows, queries the OS directly via GetSystemMetrics so the value always
    /// reflects the actual screen attached to this machine.
    /// Falls back to 1920x1080 on non-Windows platforms (e.g. Linux CI agents without
    /// a display server) where a physical screen may not be available.
    /// </summary>
    private static ViewportSize GetPrimaryScreenViewport()
    {
        if (OperatingSystem.IsWindows())
        {
            int w = GetSystemMetrics(0); // SM_CXSCREEN
            int h = GetSystemMetrics(1); // SM_CYSCREEN
            if (w > 0 && h > 0)
            {
                LoggerService.Instance.Information(
                    "Headless viewport resolved from primary screen: {W}x{H}", w, h);
                return new ViewportSize { Width = w, Height = h };
            }
        }

        LoggerService.Instance.Warning(
            "Could not resolve primary screen resolution; defaulting to 1920x1080.");
        return new ViewportSize { Width = 1920, Height = 1080 };
    }

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);
}
