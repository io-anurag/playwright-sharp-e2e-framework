using Microsoft.Playwright;

namespace AutomationFramework.Core.Extensions;

/// <summary>
/// Extension methods that add higher-level, retry-aware operations on top of
/// Playwright's <see cref="IPage"/> API.
/// Keeps page objects thin by moving reusable interaction logic here.
/// </summary>
public static class PageExtensions
{
    /// <summary>
    /// Clicks an element identified by <paramref name="selector"/> and
    /// waits for the network to be idle afterward.
    /// </summary>
    public static async Task ClickAndWaitAsync(this IPage page, string selector, int timeout = 30_000)
    {
        await page.ClickAsync(selector, new PageClickOptions { Timeout = timeout });
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Fills an input field and waits briefly for any reactive UI to settle.
    /// </summary>
    public static async Task FillAndWaitAsync(this IPage page, string selector, string value, int timeout = 30_000)
    {
        await page.FillAsync(selector, value, new PageFillOptions { Timeout = timeout });
        await page.WaitForTimeoutAsync(200);
    }

    /// <summary>
    /// Waits until the page URL contains the expected fragment/path.
    /// </summary>
    public static async Task WaitForUrlContainsAsync(this IPage page, string urlFragment, int timeout = 30_000)
    {
        await page.WaitForURLAsync(
            url => url.Contains(urlFragment, StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = timeout });
    }

    /// <summary>
    /// Returns the trimmed inner text of the first element matching <paramref name="selector"/>.
    /// </summary>
    public static async Task<string> GetTextAsync(this IPage page, string selector, int timeout = 30_000)
    {
        var text = await page.InnerTextAsync(selector, new PageInnerTextOptions { Timeout = timeout });
        return text.Trim();
    }

    /// <summary>
    /// Returns true when the element is visible within the timeout, false otherwise.
    /// Never throws — safe for conditional assertions.
    /// </summary>
    public static async Task<bool> IsVisibleWithinAsync(this IPage page, string selector, int timeout = 5_000)
    {
        try
        {
            await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                State   = WaitForSelectorState.Visible,
                Timeout = timeout
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Scrolls the element into view before interacting with it.
    /// </summary>
    public static async Task ScrollAndClickAsync(this IPage page, string selector)
    {
        await page.EvaluateAsync($"document.querySelector('{selector}')?.scrollIntoView()");
        await page.ClickAsync(selector);
    }
}
