using System.IO;
using System.Text.Json;

namespace AutomationFramework.APITests;

/// <summary>
/// Provides strongly-typed access to API assertion messages stored in
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

    public static ApiUsersSection    ApiUsers    => _data.Value.ApiUsers;
    public static ApiProjectsSection ApiProjects => _data.Value.ApiProjects;

    // ── Section records ───────────────────────────────────────────────────────

    public sealed record Root(
        ApiUsersSection    ApiUsers,
        ApiProjectsSection ApiProjects);

    public sealed record ApiUsersSection(
        string NonEmpty,
        string ExactCount,
        string ByIdExists,
        string CreateIdAssigned,
        string NotFoundReturns404,
        string CreateSucceeds,
        string CreateReturns201,
        string UpdateSucceeds,
        string DeleteReturns200,
        string IdPositive,
        string NameNotEmpty,
        string EmailNotEmpty,
        string EmailValidFormat,
        string UsernameNotEmpty);

    public sealed record ApiProjectsSection(
        string UserOwnsAtLeastOne,
        string UserOneMustExist,
        string BelongToQueriedUser,
        string UserShouldHaveProjects);
}
