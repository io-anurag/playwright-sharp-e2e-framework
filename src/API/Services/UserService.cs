using AutomationFramework.API.Clients;
using AutomationFramework.API.Requests;
using AutomationFramework.API.Responses;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.API.Services;

/// <summary>
/// Business-level service for User operations.
/// Sits above <see cref="UsersApiClient"/> and adds:
///   • Domain validation before sending requests
///   • Higher-level assertions (e.g., "create and confirm exists")
///   • Orchestration of multiple API calls into one action
///
/// Tests call UserService, not UsersApiClient directly.
/// Design Pattern: Service Layer / Facade
/// </summary>
public sealed class UserService : IDisposable
{
    private readonly UsersApiClient _client;

    public UserService(string apiBaseUrl, int timeoutMs = 30_000)
    {
        _client = new UsersApiClient(apiBaseUrl, timeoutMs);
    }

    public async Task<IList<UserResponse>> GetAllUsersAsync()
    {
        LoggerService.Instance.Information("Fetching all users");
        var response = await _client.GetAllUsersAsync();
        return response.Data ?? [];
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        LoggerService.Instance.Information("Fetching user with ID: {Id}", id);
        var response = await _client.GetUserByIdAsync(id);
        return response.IsSuccessful ? response.Data : null;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        LoggerService.Instance.Information("Creating user: {Username}", request.Username);
        var response = await _client.CreateUserAsync(request);

        if (!response.IsSuccessful)
            throw new InvalidOperationException(
                $"Failed to create user. Status: {(int)response.StatusCode} — {response.ErrorMessage}");

        return response.Data!;
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        LoggerService.Instance.Information("Updating user ID: {Id}", id);
        var response = await _client.UpdateUserAsync(id, request);

        if (!response.IsSuccessful)
            throw new InvalidOperationException(
                $"Failed to update user {id}. Status: {(int)response.StatusCode}");

        return response.Data!;
    }

    public async Task DeleteUserAsync(int id)
    {
        LoggerService.Instance.Information("Deleting user ID: {Id}", id);
        var response = await _client.DeleteUserAsync(id);

        if (!response.IsSuccessful)
            throw new InvalidOperationException(
                $"Failed to delete user {id}. Status: {(int)response.StatusCode}");
    }

    public async Task<UserResponse?> FindUserByEmailAsync(string email)
    {
        var users = await GetAllUsersAsync();
        return users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose() => _client.Dispose();
}
