using AutomationFramework.Business.UIFlows;
using AutomationFramework.UITests;
using FluentAssertions;
using NUnit.Framework;

namespace AutomationFramework.UITests.Regression;

/// <summary>
/// Regression tests for OrangeHRM Admin → User Management → System Users.
/// Target: https://opensource-demo.orangehrmlive.com/web/index.php/admin/viewSystemUsers
/// </summary>
[TestFixture]
[Category("Regression")]
[Category("UI")]
[Category("UserManagement")]
public sealed class UserManagementTests : BaseUITest
{
    private LoginFlow          _loginFlow     = null!;
    private UserManagementFlow _userMgmtFlow  = null!;

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _loginFlow    = new LoginFlow(Page, BaseUrl);
        _userMgmtFlow = new UserManagementFlow(Page, BaseUrl);

        // Pre-condition: log in as Admin before each test
        await _loginFlow.LoginAsync("Admin", "admin123");
    }

    [TearDown]
    public override async Task TearDown()
    {
        try { await _loginFlow.LogoutAsync(); } catch { /* best-effort logout */ }
        await base.TearDown();
    }

    [Test]
    [Description("Verifies the System Users page loads and displays at least one user row.")]
    public async Task SystemUsers_PageLoads_WithUserRecords()
    {
        Reporter.LogInfo("Navigating to System Users page");
        await _userMgmtFlow.GoToUsersPageAsync();

        var isLoaded = await _userMgmtFlow.IsUsersPageLoadedAsync();
        isLoaded.Should().BeTrue(ValidationMessages.UserManagement.TableVisible);

        var rowCount = await _userMgmtFlow.GetUserCountAsync();
        rowCount.Should().BeGreaterThan(0, ValidationMessages.UserManagement.HasAtLeastOneUser);

        Reporter.LogInfo("System Users page loaded with {Count} user rows", rowCount);
    }

    [Test]
    [Description("Verifies that searching for 'Admin' returns at least one matching result.")]
    public async Task SearchByUsername_Admin_ReturnsResults()
    {
        Reporter.LogInfo("Searching for username 'Admin'");
        var matchCount = await _userMgmtFlow.SearchForUserAsync("Admin");

        matchCount.Should().BeGreaterThan(0,
            ValidationMessages.UserManagement.SearchReturnsAdminResult);

        Reporter.LogInfo("Search returned {Count} rows for 'Admin'", matchCount);
    }

    [Test]
    [Description("Verifies the records-found label text matches the number of visible rows.")]
    public async Task RecordsFound_Label_Matches_VisibleRowCount()
    {
        Reporter.LogInfo("Verifying records-found label against visible rows");
        await _userMgmtFlow.GoToUsersPageAsync();

        var rowCount   = await _userMgmtFlow.GetUserCountAsync();
        var recordText = await _userMgmtFlow.GetRecordsFoundTextAsync();

        recordText.Should().Contain(rowCount.ToString(),
            ValidationMessages.UserManagement.RecordsFoundMatchesRows);

        Reporter.LogInfo("Row count: {Rows} | Records text: '{Text}'", rowCount, recordText);
    }

    [Test]
    [Description("Verifies that clicking the Add button navigates to the Add User form.")]
    public async Task AddButton_NavigatesTo_AddUserForm()
    {
        Reporter.LogInfo("Clicking Add button on System Users page");
        await _userMgmtFlow.GoToUsersPageAsync();

        await Page.ClickAsync("button:has-text('Add')");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        Page.Url.Should().Contain("saveSystemUser",
            ValidationMessages.UserManagement.AddButtonNavigatesToForm);

        Reporter.LogInfo("Add User form URL: {Url}", Page.Url);
    }
}
