using Newtonsoft.Json;

namespace AutomationFramework.API.Responses;

/// <summary>
/// Represents a project returned by the API.
/// Maps to JSONPlaceholder /posts response structure.
/// </summary>
public sealed class ProjectResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("userId")]
    public int UserId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("body")]
    public string Body { get; set; } = string.Empty;
}

/// <summary>
/// Generic envelope used when the API wraps the response body.
/// </summary>
public sealed class ApiResponse<T>
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("data")]
    public T? Data { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("errors")]
    public List<string>? Errors { get; set; }
}
