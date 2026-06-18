using AutomationFramework.Core.Logging;
using AutomationFramework.UI.Locators;
using Microsoft.Playwright;

namespace AutomationFramework.UI.Pages;

/// <summary>
/// Encapsulates interactions with the Users management page.
/// </summary>
public sealed class UsersPage : BasePage
{
    public UsersPage(IPage page) : base(page) { }

    // ── Page actions ──────────────────────────────────────────────────────────

    public async Task<bool> IsUsersPageLoadedAsync() =>
        await IsVisibleAsync(UsersLocators.UsersTable);

    public async Task ClickAddUserButtonAsync()
    {
        LoggerService.Instance.Debug("Clicking Add User button");
        await ClickAsync(UsersLocators.AddUserButton);
        await WaitForNetworkIdleAsync();
    }

    // Kept for backward compatibility with UserManagementFlow
    public Task ClickCreateUserButtonAsync() => ClickAddUserButtonAsync();

    public async Task SearchForUserAsync(string username)
    {
        LoggerService.Instance.Debug("Searching for user: {Username}", username);
        // Use nth(1) — index 0 is the sidebar search; index 1 is the username filter.
        // Playwright's Locator.FillAsync fires the Vue.js input event reliably.
        var usernameInput = Page.Locator("input.oxd-input").Nth(1);
        await usernameInput.FillAsync(username);
        // Press Enter to submit (the Search button is sometimes obscured by overlays)
        await usernameInput.PressAsync("Enter");
        await WaitForNetworkIdleAsync();
    }

    public async Task<int> GetUserCountAsync()
    {
        // Parse "(18) Records Found" — more reliable than counting DOM rows,
        // which change structure between initial list and search-result views.
        try
        {
            var text = await GetTextAsync(UsersLocators.RecordsFoundText, timeout: 8_000);
            var match = System.Text.RegularExpressions.Regex.Match(text, @"\((\d+)\)");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }
        catch
        {
            // Fallback: count DOM rows directly
            var rows = await Page.QuerySelectorAllAsync(".oxd-table-body .oxd-table-row");
            return rows.Count;
        }
    }

    public async Task<string> GetRecordsCountTextAsync() =>
        await GetTextAsync(UsersLocators.RecordsFoundText);

    public async Task ClickEditForUserAsync(string username)
    {
        LoggerService.Instance.Debug("Clicking Edit for user: {Username}", username);
        await ClickAsync(UsersLocators.EditButtonForUser(username));
        await WaitForNetworkIdleAsync();
    }

    public async Task ClickDeleteForUserAsync(string username)
    {
        LoggerService.Instance.Debug("Clicking Delete for user: {Username}", username);
        await ClickAsync(UsersLocators.DeleteButtonForUser(username));
    }

    public async Task ConfirmDeleteAsync()
    {
        await ClickAsync(UsersLocators.DeleteConfirmButton);
        await WaitForNetworkIdleAsync();
    }

    public async Task<bool> IsSuccessAlertVisibleAsync() =>
        await IsVisibleAsync(UsersLocators.SuccessToast);

    public async Task<string> GetSuccessAlertTextAsync() =>
        await GetTextAsync(UsersLocators.SuccessToast);

    // Legacy stubs — kept so UserManagementFlow compiles unchanged
    public Task FillCreateUserFormAsync(string firstName, string lastName, string email) =>
        Task.CompletedTask;

    public Task SubmitUserFormAsync() => Task.CompletedTask;
}
