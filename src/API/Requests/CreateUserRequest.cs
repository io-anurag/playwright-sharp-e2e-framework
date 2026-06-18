using Newtonsoft.Json;

namespace AutomationFramework.API.Requests;

/// <summary>
/// Request model for creating a new user.
/// Properties map directly to the API contract — use JsonProperty for
/// snake_case / camelCase JSON keys if the API requires them.
/// </summary>
public sealed class CreateUserRequest
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

    [JsonProperty("address")]
    public AddressRequest? Address { get; set; }

    [JsonProperty("company")]
    public CompanyRequest? Company { get; set; }
}

public sealed class AddressRequest
{
    [JsonProperty("street")]  public string Street  { get; set; } = string.Empty;
    [JsonProperty("suite")]   public string? Suite  { get; set; }
    [JsonProperty("city")]    public string City    { get; set; } = string.Empty;
    [JsonProperty("zipcode")] public string ZipCode { get; set; } = string.Empty;
}

public sealed class CompanyRequest
{
    [JsonProperty("name")]        public string Name        { get; set; } = string.Empty;
    [JsonProperty("catchPhrase")] public string? CatchPhrase { get; set; }
    [JsonProperty("bs")]          public string? Bs          { get; set; }
}
