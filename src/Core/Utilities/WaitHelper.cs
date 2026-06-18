using Microsoft.Playwright;

namespace AutomationFramework.Core.Utilities;

/// <summary>
/// Playwright-aware wait helpers that supplement the built-in waiting strategies.
/// Prefer these over <c>Task.Delay</c> — they poll actual DOM state rather than
/// sleeping unconditionally.
/// </summary>
public static class WaitHelper
{
    /// <summary>Waits for the page to reach NetworkIdle load state.</summary>
    public static async Task WaitForNetworkIdleAsync(IPage page, int timeout = 30_000)
    {
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle,
            new PageWaitForLoadStateOptions { Timeout = timeout });
    }

    /// <summary>Waits until an element is visible.</summary>
    public static async Task WaitForVisibleAsync(IPage page, string selector, int timeout = 30_000)
    {
        await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State   = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    /// <summary>Waits until an element is hidden or detached from the DOM.</summary>
    public static async Task WaitForHiddenAsync(IPage page, string selector, int timeout = 30_000)
    {
        await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
        {
            State   = WaitForSelectorState.Hidden,
            Timeout = timeout
        });
    }

    /// <summary>Waits for a network response that matches <paramref name="urlPattern"/>.</summary>
    public static async Task<IResponse> WaitForResponseAsync(
        IPage page, string urlPattern, Func<Task> triggerAction, int timeout = 30_000)
    {
        return await page.RunAndWaitForResponseAsync(
            triggerAction,
            response => response.Url.Contains(urlPattern),
            new PageRunAndWaitForResponseOptions { Timeout = timeout });
    }

    /// <summary>
    /// Pauses for a fixed number of milliseconds.
    /// Use sparingly — prefer DOM-based waits where possible.
    /// </summary>
    public static Task HardWaitAsync(int milliseconds) =>
        Task.Delay(milliseconds);
}
