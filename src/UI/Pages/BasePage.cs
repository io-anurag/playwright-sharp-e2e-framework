using AutomationFramework.Core.Logging;
using AutomationFramework.Core.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.UI.Pages;

/// <summary>
/// Base class for all Page Objects.
/// Holds a reference to the active Playwright page and provides common
/// navigation and interaction helpers.
///
/// Design Rules (enforced by convention):
///   • Pages contain LOCATORS and ACTIONS only.
///   • Pages must NOT contain assertions.
///   • Pages must NOT call other pages directly — use Business Flows instead.
/// </summary>
public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    // ── Common interactions ───────────────────────────────────────────────────

    protected async Task ClickAsync(string selector, int timeout = 30_000)
    {
        LoggerService.Instance.Debug("Clicking element: {Selector}", selector);
        await Page.ClickAsync(selector, new PageClickOptions { Timeout = timeout });
    }

    protected async Task FillAsync(string selector, string value, int timeout = 30_000)
    {
        LoggerService.Instance.Debug("Filling '{Selector}' with value", selector);
        await Page.FillAsync(selector, value, new PageFillOptions { Timeout = timeout });
    }

    protected async Task<string> GetTextAsync(string selector, int timeout = 30_000)
    {
        var text = await Page.InnerTextAsync(selector, new PageInnerTextOptions { Timeout = timeout });
        return text.Trim();
    }

    protected async Task<bool> IsVisibleAsync(string selector, int timeout = 5_000)
    {
        try
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                State   = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            return true;
        }
        catch { return false; }
    }

    protected async Task WaitForUrlAsync(string urlFragment, int timeout = 30_000)
    {
        await Page.WaitForURLAsync(
            url => url.Contains(urlFragment, StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = timeout });
    }

    protected async Task WaitForSelectorAsync(string selector, int timeout = 30_000)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State   = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    protected async Task NavigateToAsync(string url)
    {
        LoggerService.Instance.Information("Navigating to: {Url}", url);
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }

    protected async Task WaitForNetworkIdleAsync() =>
        await WaitHelper.WaitForNetworkIdleAsync(Page);

    protected async Task SelectOptionAsync(string selector, string value)
    {
        await Page.SelectOptionAsync(selector, value);
    }

    protected async Task<string> GetAttributeAsync(string selector, string attribute) =>
        await Page.GetAttributeAsync(selector, attribute) ?? string.Empty;

    protected async Task PressKeyAsync(string selector, string key) =>
        await Page.PressAsync(selector, key);
}
