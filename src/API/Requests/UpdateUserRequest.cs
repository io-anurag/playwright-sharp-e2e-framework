using Newtonsoft.Json;

namespace AutomationFramework.API.Requests;

/// <summary>
/// Request model for fully replacing an existing user (HTTP PUT).
/// All fields are required for a PUT — use a partial object for PATCH.
/// </summary>
public sealed class UpdateUserRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("website")]
    public string? Website { get; set; }
}

/// <summary>
/// Request model for creating a new project.
/// </summary>
public sealed class CreateProjectRequest
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("body")]
    public string Body { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public int UserId { get; set; }
}

/// <summary>
/// Request model for updating an existing project.
/// </summary>
public sealed class UpdateProjectRequest
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("body")]
    public string Body { get; set; } = string.Empty;

    [JsonProperty("userId")]
    public int UserId { get; set; }
}
