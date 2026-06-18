using Microsoft.Playwright;

namespace AutomationFramework.UI.Components;

/// <summary>
/// Represents the application header bar containing the logo, navigation,
/// and user account menu.
/// </summary>
public sealed class HeaderComponent : BaseComponent
{
    private const string LogoSelector       = "[data-testid='logo'], .app-logo, header .logo";
    private const string UserMenuSelector   = "[data-testid='user-menu'], .user-menu-trigger, #user-menu";
    private const string LogoutSelector     = "[data-testid='logout'], a:has-text('Logout'), #logout-link";
    private const string ProfileSelector    = "[data-testid='profile'], a:has-text('Profile'), #profile-link";
    private const string NotifSelector      = "[data-testid='notifications'], .notifications-icon, #notifications";

    public HeaderComponent(IPage page) : base(page) { }

    public async Task<bool> IsHeaderVisibleAsync() => await IsVisibleAsync(LogoSelector);

    public async Task OpenUserMenuAsync() => await ClickAsync(UserMenuSelector);

    public async Task ClickLogoutAsync()
    {
        await OpenUserMenuAsync();
        await ClickAsync(LogoutSelector);
    }

    public async Task ClickProfileAsync()
    {
        await OpenUserMenuAsync();
        await ClickAsync(ProfileSelector);
    }

    public async Task<bool> IsNotificationIconVisibleAsync() =>
        await IsVisibleAsync(NotifSelector);

    public async Task ClickNotificationsAsync() => await ClickAsync(NotifSelector);
}
