using AutomationFramework.Business.UIFlows;
using AutomationFramework.UITests;
using FluentAssertions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AutomationFramework.UITests.Smoke;

/// <summary>
/// Smoke tests for OrangeHRM Login.
/// Target: https://opensource-demo.orangehrmlive.com/web/index.php/auth/login
/// Default credentials: Admin / admin123
/// </summary>
[TestFixture]
[Category("Smoke")]
[Category("UI")]
[Category("Login")]
public sealed class LoginSmokeTests : BaseUITest
{
    private LoginFlow _loginFlow = null!;

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _loginFlow = new LoginFlow(Page, BaseUrl);
    }

    [Test]
    [Description("Verifies the OrangeHRM login page loads at the correct URL with all form elements visible.")]
    public async Task LoginPage_IsAccessible_WithFormElements()
    {
        Reporter.LogInfo("Navigating to OrangeHRM login page");
        await Page.GotoAsync($"{BaseUrl}/web/index.php/auth/login",
            new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        Page.Url.Should().Contain("/auth/login", ValidationMessages.Login.UrlContainsAuthLogin);
        (await Page.IsVisibleAsync("input[name='username']")
            ).Should().BeTrue(ValidationMessages.Login.UsernameInputVisible);
        (await Page.IsVisibleAsync("input[name='password']")
            ).Should().BeTrue(ValidationMessages.Login.PasswordInputVisible);
        (await Page.IsVisibleAsync("button[type='submit']")
            ).Should().BeTrue(ValidationMessages.Login.SubmitButtonVisible);

        Reporter.LogInfo("Login page verified at: {Url}", Page.Url);
    }

    [Test]
    [Description("Verifies that the Admin user logs in with valid credentials and lands on the Dashboard.")]
    public async Task ValidAdmin_CanLogin_AndSeeDashboard()
    {
        Reporter.LogInfo("Testing login with Admin / admin123");

        var dashboard = await _loginFlow.LoginAsync("Admin", "admin123");

        var isLoaded = await dashboard.IsDashboardLoadedAsync();
        isLoaded.Should().BeTrue(ValidationMessages.Login.DashboardHeadingVisible);

        var username = await dashboard.GetLoggedInUsernameAsync();
        username.Should().NotBeNullOrWhiteSpace(ValidationMessages.Login.LoggedInUsernameVisible);

        Page.Url.Should().Contain("/dashboard/index",
            ValidationMessages.Login.SuccessRedirectsToDashboard);

        Reporter.LogInfo("Dashboard loaded. Logged in as: {User}", username);
    }

    [Test]
    [Description("Verifies that wrong credentials show OrangeHRM's 'Invalid credentials' error.")]
    public async Task InvalidCredentials_ShowErrorMessage()
    {
        Reporter.LogInfo("Testing invalid credentials scenario");

        var hasError = await _loginFlow.TryLoginWithInvalidCredentialsAsync(
            "wrong_user_xyz", "BadPassword999!");

        hasError.Should().BeTrue(ValidationMessages.Login.ErrorAlertVisible);

        var errorText = await _loginFlow.GetLoginErrorMessageAsync();
        errorText.Should().Contain("Invalid credentials",
            ValidationMessages.Login.InvalidCredentialsText);

        Reporter.LogInfo("Error message shown: {Error}", errorText);
    }
}
