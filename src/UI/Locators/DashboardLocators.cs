namespace AutomationFramework.UI.Locators;

/// <summary>
/// CSS selectors for the OrangeHRM Dashboard page.
/// Verified against https://opensource-demo.orangehrmlive.com/web/index.php/dashboard/index
/// </summary>
public static class DashboardLocators
{
    // Top bar
    public const string PageHeading      = ".oxd-topbar-header-breadcrumb-module";
    public const string LoggedInUser     = ".oxd-userdropdown-name";
    public const string UserMenuTrigger  = ".oxd-userdropdown-tab";
    public const string LogoutLink       = ".oxd-userdropdown-link:has-text('Logout')";
    public const string MainContent      = ".oxd-layout-context";

    // Side navigation
    public const string NavMenuItem      = ".oxd-main-menu-item";
    public const string SidebarSearch    = "input.oxd-input[placeholder='Search']";

    /// <summary>Selector for a nav menu link by its visible text.</summary>
    public static string NavMenuItemByText(string text) =>
        $".oxd-main-menu-item:has-text('{text}')";
}
