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
    private IBrowser?        _browser;
    private IBrowserContext? _context;
    private IPage?           _page;

    public IPage           Page    => _page    ?? throw new DriverException("Page not initialised. Call InitializeAsync() first.");
    public IBrowser        Browser => _browser ?? throw new DriverException("Browser not initialised. Call InitializeAsync() first.");
    public IBrowserContext Context => _context ?? throw new DriverException("Context not initialised. Call InitializeAsync() first.");

    internal PlaywrightBrowserDriver(IPlaywright playwright, IBrowser browser)
    {
        _playwright = playwright;
        _browser    = browser;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
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
}
