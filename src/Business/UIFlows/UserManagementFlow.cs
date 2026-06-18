using AutomationFramework.Core.Logging;
using AutomationFramework.UI.Pages;
using AutomationFramework.UI.Workflows;
using Microsoft.Playwright;

namespace AutomationFramework.Business.UIFlows;

/// <summary>
/// Orchestrates user management UI flows: create, search, edit, and delete users.
/// Assumes the user is already logged in before any method is called.
/// </summary>
public sealed class UserManagementFlow
{
    private readonly IPage _page;
    private readonly UsersPage _usersPage;
    private readonly NavigationWorkflow _navigation;

    public UserManagementFlow(IPage page, string baseUrl)
    {
        _page       = page;
        _usersPage  = new UsersPage(page);
        _navigation = new NavigationWorkflow(page, baseUrl);
    }

    /// <summary>Navigates to Users and creates a new user via the UI form.</summary>
    public async Task<bool> CreateUserAsync(
        string firstName, string lastName, string email)
    {
        LoggerService.Instance.Information("Creating user via UI: {FirstName} {LastName}", firstName, lastName);

        await _navigation.GoToUsersAsync();
        await _usersPage.ClickCreateUserButtonAsync();
        await _usersPage.FillCreateUserFormAsync(firstName, lastName, email);
        await _usersPage.SubmitUserFormAsync();

        var success = await _usersPage.IsSuccessAlertVisibleAsync();
        LoggerService.Instance.Information("Create user result: {Success}", success);
        return success;
    }

    /// <summary>Searches for a user and returns the count of matching rows.</summary>
    public async Task<int> SearchForUserAsync(string searchTerm)
    {
        LoggerService.Instance.Information("Searching for user: {SearchTerm}", searchTerm);
        await _navigation.GoToUsersAsync();
        await _usersPage.SearchForUserAsync(searchTerm);
        return await _usersPage.GetUserCountAsync();
    }

    /// <summary>Deletes a user and confirms the operation via the modal.</summary>
    public async Task DeleteUserAsync(string username)
    {
        LoggerService.Instance.Information("Deleting user via UI: {Username}", username);
        await _navigation.GoToUsersAsync();
        await _usersPage.ClickDeleteForUserAsync(username);

        // Confirm the deletion modal
        var modal = new UI.Components.ModalComponent(_page);
        await modal.WaitForOpenAsync();
        await modal.ClickConfirmAsync();

        LoggerService.Instance.Information("User deleted: {Username}", username);
    }

    public async Task GoToUsersPageAsync()
    {
        LoggerService.Instance.Information("Navigating to Users page");
        await _navigation.GoToUsersAsync();
    }

    public async Task<bool> IsUsersPageLoadedAsync() =>
        await _usersPage.IsUsersPageLoadedAsync();

    public async Task<int> GetUserCountAsync()
    {
        await _navigation.GoToUsersAsync();
        return await _usersPage.GetUserCountAsync();
    }

    public async Task<string> GetRecordsFoundTextAsync()
    {
        return await _usersPage.GetRecordsCountTextAsync();
    }
}
