using AutomationFramework.Core.Logging;
using AutomationFramework.UI.Locators;
using Microsoft.Playwright;

namespace AutomationFramework.UI.Pages;

/// <summary>
/// Encapsulates all interactions with the Login page.
/// Returns <c>void</c> or primitive values — never navigates to another page itself.
/// Business logic (e.g., "log in as admin") lives in LoginFlow.
/// </summary>
public sealed class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page) { }

    // ── Page actions ──────────────────────────────────────────────────────────

    public async Task EnterUsernameAsync(string username)
    {
        LoggerService.Instance.Debug("Entering username: {Username}", username);
        await FillAsync(LoginLocators.UsernameInput, username);
    }

    public async Task EnterPasswordAsync(string password)
    {
        LoggerService.Instance.Debug("Entering password");
        await FillAsync(LoginLocators.PasswordInput, password);
    }

    public async Task ClickLoginButtonAsync()
    {
        LoggerService.Instance.Debug("Clicking login button");
        await ClickAsync(LoginLocators.LoginButton);
        await WaitForNetworkIdleAsync();
    }

    public async Task<string> GetErrorMessageAsync() =>
        await GetTextAsync(LoginLocators.ErrorMessage);

    public async Task<bool> IsErrorMessageVisibleAsync() =>
        await IsVisibleAsync(LoginLocators.ErrorMessage);

    public async Task<bool> IsLoginPageVisibleAsync() =>
        await IsVisibleAsync(LoginLocators.LoginButton);

    public async Task NavigateToAsync(string baseUrl) =>
        await NavigateToAsync($"{baseUrl.TrimEnd('/')}/login");

    /// <summary>One-shot convenience: fill credentials and click Submit.</summary>
    public async Task LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginButtonAsync();
    }
}
