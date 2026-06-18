using Newtonsoft.Json;

namespace AutomationFramework.API.Responses;

/// <summary>
/// Represents a user returned by the API.
/// Maps to JSONPlaceholder /users response structure.
/// </summary>
public sealed class UserResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

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
    public AddressResponse? Address { get; set; }

    [JsonProperty("company")]
    public CompanyResponse? Company { get; set; }
}

public sealed class AddressResponse
{
    [JsonProperty("street")]  public string Street   { get; set; } = string.Empty;
    [JsonProperty("suite")]   public string? Suite   { get; set; }
    [JsonProperty("city")]    public string City     { get; set; } = string.Empty;
    [JsonProperty("zipcode")] public string ZipCode  { get; set; } = string.Empty;
}

public sealed class CompanyResponse
{
    [JsonProperty("name")]        public string Name         { get; set; } = string.Empty;
    [JsonProperty("catchPhrase")] public string? CatchPhrase { get; set; }
    [JsonProperty("bs")]          public string? Bs          { get; set; }
}
