namespace AutomationFramework.API.Endpoints;

/// <summary>
/// Centralised endpoint path definitions for the Users resource.
/// Avoids magic strings scattered across tests and clients.
/// </summary>
public static class UserEndpoints
{
    private const string Base = "/users";

    public const string GetAll = Base;
    public const string Create = Base;

    public static string GetById(int id) => $"{Base}/{id}";
    public static string Update(int id)  => $"{Base}/{id}";
    public static string Patch(int id)   => $"{Base}/{id}";
    public static string Delete(int id)  => $"{Base}/{id}";
}
