using AutomationFramework.Core.Logging;
using Microsoft.Playwright;

namespace AutomationFramework.UI.Components;

/// <summary>
/// Base class for reusable UI components (header, grid, modal, etc.).
/// Components are smaller than pages — they represent one logical region of the UI
/// and may be embedded in multiple page objects.
/// </summary>
public abstract class BaseComponent
{
    protected readonly IPage Page;

    protected BaseComponent(IPage page)
    {
        Page = page;
    }

    protected async Task ClickAsync(string selector, int timeout = 30_000)
    {
        LoggerService.Instance.Debug("[{Component}] Clicking: {Selector}", GetType().Name, selector);
        await Page.ClickAsync(selector, new PageClickOptions { Timeout = timeout });
    }

    protected async Task<string> GetTextAsync(string selector, int timeout = 10_000)
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

    protected async Task WaitForVisibleAsync(string selector, int timeout = 30_000)
    {
        await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State   = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }
}
