using AutomationFramework.Core.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace AutomationFramework.API.Clients;

/// <summary>
/// Base class for all API clients.
/// Wraps RestSharp's <see cref="RestClient"/> and adds:
///   • Centralised request building (headers, auth, base URL)
///   • Structured Serilog logging for every request/response
///   • Consistent error handling and exception mapping
///
/// Design Pattern: Template Method — subclasses only define endpoints and
/// payload shapes; this class owns the HTTP mechanics.
/// </summary>
public abstract class BaseApiClient : IDisposable
{
    private readonly RestClient _client;
    private bool _disposed;

    protected BaseApiClient(string baseUrl, int timeoutMs = 30_000)
    {
        var options = new RestClientOptions(baseUrl)
        {
            MaxTimeout = timeoutMs,
            ThrowOnAnyError = false
        };
        _client = new RestClient(options);
    }

    // ── HTTP verb helpers ─────────────────────────────────────────────────────

    protected async Task<RestResponse<T>> GetAsync<T>(string endpoint,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? queryParams = null)
    {
        var request = BuildRequest(endpoint, Method.Get, null, headers, queryParams);
        return await ExecuteAsync<T>(request);
    }

    protected async Task<RestResponse<T>> PostAsync<T>(string endpoint, object? body,
        Dictionary<string, string>? headers = null)
    {
        var request = BuildRequest(endpoint, Method.Post, body, headers);
        return await ExecuteAsync<T>(request);
    }

    protected async Task<RestResponse<T>> PutAsync<T>(string endpoint, object? body,
        Dictionary<string, string>? headers = null)
    {
        var request = BuildRequest(endpoint, Method.Put, body, headers);
        return await ExecuteAsync<T>(request);
    }

    protected async Task<RestResponse<T>> PatchAsync<T>(string endpoint, object? body,
        Dictionary<string, string>? headers = null)
    {
        var request = BuildRequest(endpoint, Method.Patch, body, headers);
        return await ExecuteAsync<T>(request);
    }

    protected async Task<RestResponse> DeleteAsync(string endpoint,
        Dictionary<string, string>? headers = null)
    {
        var request = BuildRequest(endpoint, Method.Delete, null, headers);
        return await ExecuteRawAsync(request);
    }

    // ── Auth helpers ──────────────────────────────────────────────────────────

    protected void SetBearerToken(string token) =>
        _client.AddDefaultHeader("Authorization", $"Bearer {token}");

    protected void SetApiKey(string headerName, string apiKey) =>
        _client.AddDefaultHeader(headerName, apiKey);

    // ── Private implementation ────────────────────────────────────────────────

    private static RestRequest BuildRequest(
        string endpoint,
        Method method,
        object? body,
        Dictionary<string, string>? headers,
        Dictionary<string, string>? queryParams = null)
    {
        var request = new RestRequest(endpoint, method);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");

        if (headers is not null)
            foreach (var (key, value) in headers)
                request.AddHeader(key, value);

        if (queryParams is not null)
            foreach (var (key, value) in queryParams)
                request.AddQueryParameter(key, value);

        if (body is not null)
            request.AddJsonBody(body);

        return request;
    }

    private async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request)
    {
        LogRequest(request);
        var response = await _client.ExecuteAsync<T>(request);
        LogResponse(response);
        return response;
    }

    private async Task<RestResponse> ExecuteRawAsync(RestRequest request)
    {
        LogRequest(request);
        var response = await _client.ExecuteAsync(request);
        LogResponse(response);
        return response;
    }

    private static void LogRequest(RestRequest request)
    {
        LoggerService.Instance.Information(
            "API Request → {Method} {Resource}",
            request.Method, request.Resource);

        if (request.Parameters.Any(p => p.Type == ParameterType.RequestBody))
        {
            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            LoggerService.Instance.Debug("Request Body: {Body}", body?.Value);
        }
    }

    private static void LogResponse(RestResponseBase response)
    {
        LoggerService.Instance.Information(
            "API Response ← {StatusCode} | {Url}",
            (int)response.StatusCode, response.ResponseUri);

        if (!string.IsNullOrWhiteSpace(response.Content))
            LoggerService.Instance.Debug("Response Body: {Body}",
                response.Content?.Truncate(500));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _client.Dispose();
    }
}

file static class StringTruncateExtension
{
    public static string? Truncate(this string? value, int maxLength)
        => value is null || value.Length <= maxLength ? value : value[..maxLength] + "…";
}
