using AutomationFramework.Core.Constants;
using AutomationFramework.Core.Enums;
using AutomationFramework.Core.Exceptions;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;
using Microsoft.Playwright;
using CoreBrowserType = AutomationFramework.Core.Enums.BrowserType;

namespace AutomationFramework.Core.Drivers;

/// <summary>
/// Factory responsible for creating browser instances.
/// Implements the Factory Pattern — callers only know about IBrowserDriver;
/// the concrete Playwright implementation is an internal detail.
///
/// To add Selenium support, create a SeleniumBrowserDriver implementing
/// IBrowserDriver and add a new branch to CreateAsync().
/// </summary>
public static class BrowserFactory
{
    /// <summary>
    /// Creates and returns an uninitialised <see cref="IBrowserDriver"/>.
    /// Call <c>InitializeAsync()</c> after creation to open the page.
    /// </summary>
    /// <param name="browserName">Browser identifier (chromium/chrome/firefox/edge/webkit).</param>
    /// <param name="headless">Whether to run without a visible window.</param>
    public static async Task<IBrowserDriver> CreateAsync(
        string browserName,
        bool   headless = true)
    {
        var playwright = await Playwright.CreateAsync();

        LoggerService.Instance.Information(
            "Launching browser: {Browser} | Headless: {Headless}", browserName, headless);

        IBrowser browser = browserName.Trim().ToLowerInvariant() switch
        {
            BrowserConstants.Chrome =>
                await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args     = ["--no-sandbox", "--disable-dev-shm-usage"],
                    Channel  = "chrome"
                }),

            BrowserConstants.Edge =>
                await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args     = ["--no-sandbox", "--disable-dev-shm-usage"],
                    Channel  = "msedge"
                }),

            BrowserConstants.Firefox =>
                await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args     = ["--no-sandbox", "--disable-dev-shm-usage"]
                }),

            BrowserConstants.WebKit =>
                await playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless
                }),

            _ =>
                await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args     = ["--no-sandbox", "--disable-dev-shm-usage"]
                })
        };

        if (!new[] { BrowserConstants.Chromium, BrowserConstants.Chrome, BrowserConstants.Firefox,
                     BrowserConstants.Edge, BrowserConstants.WebKit }
                .Contains(browserName.Trim().ToLowerInvariant()))
        {
            LoggerService.Instance.Warning(
                "Unknown browser '{Browser}'. Defaulting to Chromium.", browserName);
        }

        return new PlaywrightBrowserDriver(playwright, browser);
    }

    /// <summary>Overload that accepts the <see cref="CoreBrowserType"/> enum.</summary>
    public static Task<IBrowserDriver> CreateAsync(CoreBrowserType browserType, bool headless = true)
        => CreateAsync(browserType.ToString().ToLowerInvariant(), headless);
}
