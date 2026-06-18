using AutomationFramework.API.Requests;
using AutomationFramework.API.Responses;
using AutomationFramework.API.Services;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Business.APIFacades;

/// <summary>
/// Facade that exposes high-level user API operations to tests.
/// Combines UserService calls and adds test-friendly result types.
/// Design Pattern: Facade — simplifies the API surface for test authors.
/// </summary>
public sealed class UserApiFacade : IDisposable
{
    private readonly UserService _userService;

    public UserApiFacade(string apiBaseUrl, int timeoutMs = 30_000)
    {
        _userService = new UserService(apiBaseUrl, timeoutMs);
    }

    public async Task<IList<UserResponse>> GetAllUsersAsync() =>
        await _userService.GetAllUsersAsync();

    public async Task<UserResponse?> GetUserByIdAsync(int id) =>
        await _userService.GetUserByIdAsync(id);

    public async Task<UserResponse> CreateUserAsync(string name, string username, string email)
    {
        LoggerService.Instance.Information("API Facade: Creating user {Username}", username);
        var request = new CreateUserRequest
        {
            Name     = name,
            Username = username,
            Email    = email
        };
        return await _userService.CreateUserAsync(request);
    }

    public async Task<UserResponse> UpdateUserAsync(int id, string name, string email)
    {
        LoggerService.Instance.Information("API Facade: Updating user {Id}", id);
        var request = new UpdateUserRequest
        {
            Name  = name,
            Email = email,
            Username = string.Empty
        };
        return await _userService.UpdateUserAsync(id, request);
    }

    public async Task DeleteUserAsync(int id) =>
        await _userService.DeleteUserAsync(id);

    public async Task<UserResponse?> FindByEmailAsync(string email) =>
        await _userService.FindUserByEmailAsync(email);

    public void Dispose() => _userService.Dispose();
}
