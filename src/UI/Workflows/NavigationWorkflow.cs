using AutomationFramework.Core.Logging;
using Microsoft.Playwright;

namespace AutomationFramework.UI.Workflows;

/// <summary>
/// Low-level navigation helpers used by Business Flows.
/// Responsible only for URL-based navigation, not business logic.
/// </summary>
public sealed class NavigationWorkflow
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public NavigationWorkflow(IPage page, string baseUrl)
    {
        _page    = page;
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task GoToLoginAsync()
    {
        LoggerService.Instance.Information("Navigating to login page");
        await _page.GotoAsync($"{_baseUrl}/web/index.php/auth/login",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task GoToDashboardAsync()
    {
        LoggerService.Instance.Information("Navigating to dashboard");
        await _page.GotoAsync($"{_baseUrl}/web/index.php/dashboard/index",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task GoToUsersAsync()
    {
        LoggerService.Instance.Information("Navigating to users page");
        await _page.GotoAsync($"{_baseUrl}/web/index.php/admin/viewSystemUsers",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task GoToProjectsAsync()
    {
        LoggerService.Instance.Information("Navigating to projects page");
        await _page.GotoAsync($"{_baseUrl}/web/index.php/project/viewProjectList",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task GoToAsync(string relativeUrl)
    {
        var fullUrl = relativeUrl.StartsWith("http") ? relativeUrl : $"{_baseUrl}/{relativeUrl.TrimStart('/')}";
        LoggerService.Instance.Information("Navigating to: {Url}", fullUrl);
        await _page.GotoAsync(fullUrl,
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public string CurrentUrl => _page.Url;
}
