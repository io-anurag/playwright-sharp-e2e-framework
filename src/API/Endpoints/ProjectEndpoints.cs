namespace AutomationFramework.API.Endpoints;

/// <summary>
/// Endpoint path definitions for the Projects resource.
/// </summary>
public static class ProjectEndpoints
{
    private const string Base = "/posts"; // JSONPlaceholder uses /posts as projects equivalent

    public const string GetAll = Base;
    public const string Create = Base;

    public static string GetById(int id) => $"{Base}/{id}";
    public static string Update(int id)  => $"{Base}/{id}";
    public static string Delete(int id)  => $"{Base}/{id}";
}
