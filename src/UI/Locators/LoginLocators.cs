namespace AutomationFramework.UI.Locators;

/// <summary>
/// CSS selectors for the OrangeHRM Login page.
/// Verified against https://opensource-demo.orangehrmlive.com/web/index.php/auth/login
/// </summary>
public static class LoginLocators
{
    public const string UsernameInput    = "input[name='username']";
    public const string PasswordInput    = "input[name='password']";
    public const string LoginButton      = "button[type='submit']";
    public const string ErrorMessage     = ".oxd-alert-content-text";
    public const string ForgotPasswordLink = "p.orangehrm-login-forgot-header";
    public const string LoginPageHeading = ".orangehrm-login-title";
}
