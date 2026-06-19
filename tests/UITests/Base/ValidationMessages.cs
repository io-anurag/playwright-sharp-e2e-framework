using System.IO;
using System.Text.Json;

namespace AutomationFramework.UITests;

/// <summary>
/// Provides strongly-typed access to UI assertion messages stored in
/// tests/TestData/Json/validation-messages.json.
/// </summary>
public static class ValidationMessages
{
    private static readonly Lazy<Root> _data = new(() =>
    {
        var path = Path.Combine(
            AppContext.BaseDirectory, "TestData", "Json", "validation-messages.json");
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Root>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    });

    public static LoginSection      Login          => _data.Value.Login;
    public static UserMgmtSection   UserManagement => _data.Value.UserManagement;
    public static NavigationSection Navigation     => _data.Value.Navigation;

    // ── Section records ───────────────────────────────────────────────────────

    public sealed record Root(
        LoginSection      Login,
        UserMgmtSection   UserManagement,
        NavigationSection Navigation);

    public sealed record LoginSection(
        string UrlContainsAuthLogin,
        string UsernameInputVisible,
        string PasswordInputVisible,
        string SubmitButtonVisible,
        string DashboardHeadingVisible,
        string LoggedInUsernameVisible,
        string SuccessRedirectsToDashboard,
        string ErrorAlertVisible,
        string InvalidCredentialsText);

    public sealed record UserMgmtSection(
        string TableVisible,
        string HasAtLeastOneUser,
        string SearchReturnsAdminResult,
        string RecordsFoundMatchesRows,
        string AddButtonNavigatesToForm);

    public sealed record NavigationSection(
        string SystemUsersUrl,
        string SystemUsersTableRendered,
        string SystemUsersHasRows,
        string PimUrl,
        string DashboardUrl,
        string DashboardHeading,
        string LogoutRedirectsToLogin);
}
