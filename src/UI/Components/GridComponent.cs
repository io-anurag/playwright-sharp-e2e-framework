using Microsoft.Playwright;

namespace AutomationFramework.UI.Components;

/// <summary>
/// Generic data-grid / table component.
/// Provides row counting, column value reading, sorting, and pagination controls.
/// </summary>
public sealed class GridComponent : BaseComponent
{
    private readonly string _tableSelector;

    public GridComponent(IPage page, string tableSelector = "table") : base(page)
    {
        _tableSelector = tableSelector;
    }

    public async Task<int> GetRowCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync($"{_tableSelector} tbody tr");
        return rows.Count;
    }

    public async Task<IReadOnlyList<string>> GetColumnValuesAsync(int columnIndex)
    {
        var cells = await Page.QuerySelectorAllAsync(
            $"{_tableSelector} tbody tr td:nth-child({columnIndex})");
        var values = new List<string>();
        foreach (var cell in cells)
            values.Add((await cell.InnerTextAsync()).Trim());
        return values;
    }

    public async Task<string> GetCellValueAsync(int rowIndex, int columnIndex)
    {
        var selector = $"{_tableSelector} tbody tr:nth-child({rowIndex}) td:nth-child({columnIndex})";
        return await GetTextAsync(selector);
    }

    public async Task ClickCellAsync(int rowIndex, int columnIndex)
    {
        var selector = $"{_tableSelector} tbody tr:nth-child({rowIndex}) td:nth-child({columnIndex})";
        await ClickAsync(selector);
    }

    public async Task ClickHeaderAsync(string columnName)
    {
        var selector = $"{_tableSelector} th:has-text('{columnName}')";
        await ClickAsync(selector);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> IsEmptyAsync()
    {
        var emptyState = $"{_tableSelector} tbody tr td.empty, {_tableSelector} .no-data";
        return await IsVisibleAsync(emptyState, 2_000) || await GetRowCountAsync() == 0;
    }

    public async Task ClickNextPageAsync()
    {
        await ClickAsync("[data-testid='next-page'], .pagination .next, button[aria-label='Next page']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickPreviousPageAsync()
    {
        await ClickAsync("[data-testid='prev-page'], .pagination .prev, button[aria-label='Previous page']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
