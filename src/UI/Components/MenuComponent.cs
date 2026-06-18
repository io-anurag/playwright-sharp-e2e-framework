using Microsoft.Playwright;

namespace AutomationFramework.UI.Components;

/// <summary>
/// Represents the sidebar / top navigation menu.
/// </summary>
public sealed class MenuComponent : BaseComponent
{
    private const string MenuContainer = "nav[data-testid='sidebar'], nav.sidebar, #sidebar-nav";

    public MenuComponent(IPage page) : base(page) { }

    public async Task<bool> IsMenuVisibleAsync() => await IsVisibleAsync(MenuContainer);

    public async Task ClickMenuItemAsync(string itemText)
    {
        var selector = $"nav a:has-text('{itemText}'), nav [data-testid='{itemText.ToLower().Replace(' ', '-')}']";
        await ClickAsync(selector);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> IsMenuItemActiveAsync(string itemText)
    {
        var selector = $"nav a:has-text('{itemText}').active, nav a[aria-current='page']:has-text('{itemText}')";
        return await IsVisibleAsync(selector);
    }

    public async Task<IReadOnlyList<string>> GetMenuItemsAsync()
    {
        var elements = await Page.QuerySelectorAllAsync($"{MenuContainer} a");
        var texts    = new List<string>();
        foreach (var el in elements)
        {
            var text = await el.InnerTextAsync();
            if (!string.IsNullOrWhiteSpace(text))
                texts.Add(text.Trim());
        }
        return texts;
    }
}
