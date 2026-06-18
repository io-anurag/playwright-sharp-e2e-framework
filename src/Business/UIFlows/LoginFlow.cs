using AutomationFramework.Core.Logging;
using AutomationFramework.UI.Pages;
using AutomationFramework.UI.Workflows;
using Microsoft.Playwright;

namespace AutomationFramework.Business.UIFlows;

/// <summary>
/// Orchestrates the login business flow.
/// Calls LoginPage and DashboardPage together so tests stay at a high level.
/// Design Pattern: Facade — hides multi-page orchestration behind one method.
/// </summary>
public sealed class LoginFlow
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    private readonly LoginPage _loginPage;
    private readonly DashboardPage _dashboardPage;
    private readonly NavigationWorkflow _navigation;

    public LoginFlow(IPage page, string baseUrl)
    {
        _page          = page;
        _baseUrl       = baseUrl;
        _loginPage     = new LoginPage(page);
        _dashboardPage = new DashboardPage(page);
        _navigation    = new NavigationWorkflow(page, baseUrl);
    }

    /// <summary>
    /// Performs a full login and asserts the dashboard is loaded.
    /// Returns the <see cref="DashboardPage"/> for further interaction.
    /// </summary>
    public async Task<DashboardPage> LoginAsync(string username, string password)
    {
        LoggerService.Instance.Information("Starting login flow for user: {Username}", username);

        await _navigation.GoToLoginAsync();
        await _loginPage.LoginAsync(username, password);

        var isDashboardLoaded = await _dashboardPage.IsDashboardLoadedAsync();
        if (!isDashboardLoaded)
        {
            var error = await _loginPage.IsErrorMessageVisibleAsync()
                ? await _loginPage.GetErrorMessageAsync()
                : "Dashboard did not load after login";

            throw new InvalidOperationException($"Login failed: {error}");
        }

        LoggerService.Instance.Information("Login successful for user: {Username}", username);
        return _dashboardPage;
    }

    /// <summary>
    /// Attempts login and returns whether the error message is displayed.
    /// Use for negative / invalid-credential test scenarios.
    /// </summary>
    public async Task<bool> TryLoginWithInvalidCredentialsAsync(string username, string password)
    {
        await _navigation.GoToLoginAsync();
        await _loginPage.LoginAsync(username, password);
        return await _loginPage.IsErrorMessageVisibleAsync();
    }

    public async Task<string> GetLoginErrorMessageAsync() =>
        await _loginPage.GetErrorMessageAsync();

    public async Task LogoutAsync()
    {
        LoggerService.Instance.Information("Logging out");
        await _dashboardPage.LogoutAsync();
    }
}
