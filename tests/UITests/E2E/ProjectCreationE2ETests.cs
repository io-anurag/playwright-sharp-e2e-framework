using AutomationFramework.Business.UIFlows;
using AutomationFramework.UITests;
using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AutomationFramework.UITests.E2E;

/// <summary>
/// End-to-End tests covering cross-module navigation and full user journeys
/// in OrangeHRM Open Source.
///
/// Scenarios:
///   - Admin logs in → navigates to every top-level module → logs out
///   - Admin logs in → opens User Management → verifies table → logs out
/// </summary>
[TestFixture]
[Category("E2E")]
[Category("UI")]
[Category("Navigation")]
public sealed class OrangeHrmNavigationE2ETests : BaseUITest
{
    private LoginFlow          _loginFlow     = null!;
    private UserManagementFlow _userMgmtFlow  = null!;

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _loginFlow    = new LoginFlow(Page, BaseUrl);
        _userMgmtFlow = new UserManagementFlow(Page, BaseUrl);

        // All E2E tests start logged in as Admin
        await _loginFlow.LoginAsync("Admin", "admin123");
    }

    [TearDown]
    public override async Task TearDown()
    {
        try { await _loginFlow.LogoutAsync(); } catch { /* best-effort */ }
        await base.TearDown();
    }

    [Test]
    [Description("E2E: Admin logs in, verifies all expected side-nav modules are present, then logs out.")]
    public async Task Admin_SideNav_ContainsAllExpectedModules()
    {
        Reporter.LogInfo("Verifying all OrangeHRM navigation modules are visible");

        // OrangeHRM OS 5.8 modules
        var expectedModules = new[]
        {
            "Admin", "PIM", "Leave", "Time", "Recruitment",
            "My Info", "Performance", "Dashboard",
            "Directory", "Maintenance", "Claim", "Buzz"
        };

        // Use the DashboardPage via the LoginFlow result
        var dashboard = new UI.Pages.DashboardPage(Page);
        var actualModules = await dashboard.GetAllMenuItemsAsync();

        foreach (var module in expectedModules)
        {
            actualModules.Should().Contain(module,
                $"the '{module}' module must appear in the side navigation");
        }

        Reporter.LogInfo("All {Count} navigation modules verified", expectedModules.Length);
    }

    [Test]
    [Description("E2E: Admin navigates to Admin → User Management → System Users and verifies the table.")]
    public async Task Admin_CanNavigate_ToUserManagement_ViaDirectUrl()
    {
        Reporter.LogInfo("Navigating to Admin > User Management > System Users");
        await _userMgmtFlow.GoToUsersPageAsync();

        // URL check
        Page.Url.Should().Contain("viewSystemUsers",
            ValidationMessages.Navigation.SystemUsersUrl);

        // Table check
        var isLoaded = await _userMgmtFlow.IsUsersPageLoadedAsync();
        isLoaded.Should().BeTrue(ValidationMessages.Navigation.SystemUsersTableRendered);

        var rowCount = await _userMgmtFlow.GetUserCountAsync();
        rowCount.Should().BeGreaterThan(0, ValidationMessages.Navigation.SystemUsersHasRows);

        Reporter.LogInfo("User Management loaded with {Count} rows", rowCount);
    }

    [Test]
    [Description("E2E: Full login → navigate to PIM module → verify URL → logout.")]
    public async Task Admin_CanNavigate_ToPimModule_AndReturnToDashboard()
    {
        Reporter.LogInfo("E2E: navigating to PIM module");
        var dashboard = new UI.Pages.DashboardPage(Page);

        await dashboard.ClickNavigationMenuAsync("PIM");

        Page.Url.Should().Contain("/pim/",
            ValidationMessages.Navigation.PimUrl);

        Reporter.LogInfo("PIM module URL: {Url}", Page.Url);

        // Navigate back to dashboard
        await dashboard.ClickNavigationMenuAsync("Dashboard");
        Page.Url.Should().Contain("/dashboard/",
            ValidationMessages.Navigation.DashboardUrl);

        Reporter.LogInfo("Returned to Dashboard: {Url}", Page.Url);
    }

    [Test]
    [Description("E2E: Full login → verify dashboard page heading → logout → verify redirect to login.")]
    public async Task Admin_FullLoginLogout_Cycle_RedirectsToLogin()
    {
        Reporter.LogInfo("Verifying full login-logout cycle");
        var dashboard = new UI.Pages.DashboardPage(Page);

        // Dashboard heading must say "Dashboard"
        var heading = await dashboard.GetPageHeadingAsync();
        heading.Should().Be("Dashboard",
            ValidationMessages.Navigation.DashboardHeading);

        // Logout
        await _loginFlow.LogoutAsync();

        // Must land back on login
        Page.Url.Should().Contain("/auth/login",
            ValidationMessages.Navigation.LogoutRedirectsToLogin);

        Reporter.LogInfo("Logout successful. Redirected to: {Url}", Page.Url);
    }
}
