using AutomationFramework.Core.Logging;
using AutomationFramework.UI.Locators;
using Microsoft.Playwright;

namespace AutomationFramework.UI.Pages;

/// <summary>
/// Encapsulates all interactions with the Dashboard / Home page.
/// </summary>
public sealed class DashboardPage : BasePage
{
    public DashboardPage(IPage page) : base(page) { }

    // ── Page actions ──────────────────────────────────────────────────────────

    public async Task<bool> IsDashboardLoadedAsync() =>
        await IsVisibleAsync(DashboardLocators.PageHeading);

    public async Task<string> GetPageHeadingAsync() =>
        await GetTextAsync(DashboardLocators.PageHeading);

    public async Task<string> GetLoggedInUsernameAsync() =>
        await GetTextAsync(DashboardLocators.LoggedInUser);

    public async Task ClickNavigationMenuAsync(string menuItem)
    {
        LoggerService.Instance.Debug("Clicking navigation menu: {MenuItem}", menuItem);
        await ClickAsync(DashboardLocators.NavMenuItemByText(menuItem));
        await WaitForNetworkIdleAsync();
    }

    public async Task LogoutAsync()
    {
        LoggerService.Instance.Information("Logging out via dashboard");
        // OrangeHRM: click user avatar to open dropdown, then click Logout
        await ClickAsync(DashboardLocators.UserMenuTrigger);
        await WaitForSelectorAsync(DashboardLocators.LogoutLink, timeout: 5_000);
        await ClickAsync(DashboardLocators.LogoutLink);
        await WaitForNetworkIdleAsync();
    }

    public async Task<bool> IsMenuItemVisibleAsync(string menuItem) =>
        await IsVisibleAsync(DashboardLocators.NavMenuItemByText(menuItem));

    public async Task<IReadOnlyList<string>> GetAllMenuItemsAsync()
    {
        var items = await Page.QuerySelectorAllAsync(DashboardLocators.NavMenuItem);
        var texts = new List<string>();
        foreach (var item in items)
            texts.Add((await item.InnerTextAsync()).Trim());
        return texts;
    }
}
