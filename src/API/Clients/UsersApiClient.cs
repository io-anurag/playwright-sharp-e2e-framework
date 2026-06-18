using AutomationFramework.API.Endpoints;
using AutomationFramework.API.Requests;
using AutomationFramework.API.Responses;
using RestSharp;

namespace AutomationFramework.API.Clients;

/// <summary>
/// API client for the Users resource.
/// Exposes typed methods for each Users endpoint — tests never construct raw requests.
/// </summary>
public sealed class UsersApiClient : BaseApiClient
{
    public UsersApiClient(string baseUrl, int timeoutMs = 30_000)
        : base(baseUrl, timeoutMs) { }

    public async Task<RestResponse<List<UserResponse>>> GetAllUsersAsync() =>
        await GetAsync<List<UserResponse>>(UserEndpoints.GetAll);

    public async Task<RestResponse<UserResponse>> GetUserByIdAsync(int id) =>
        await GetAsync<UserResponse>(UserEndpoints.GetById(id));

    public async Task<RestResponse<UserResponse>> CreateUserAsync(CreateUserRequest request) =>
        await PostAsync<UserResponse>(UserEndpoints.Create, request);

    public async Task<RestResponse<UserResponse>> UpdateUserAsync(int id, UpdateUserRequest request) =>
        await PutAsync<UserResponse>(UserEndpoints.Update(id), request);

    public async Task<RestResponse<UserResponse>> PatchUserAsync(int id, object partialUpdate) =>
        await PatchAsync<UserResponse>(UserEndpoints.Patch(id), partialUpdate);

    public async Task<RestResponse> DeleteUserAsync(int id) =>
        await DeleteAsync(UserEndpoints.Delete(id));

    public async Task<RestResponse<List<UserResponse>>> SearchUsersAsync(string searchTerm) =>
        await GetAsync<List<UserResponse>>(
            UserEndpoints.GetAll,
            queryParams: new Dictionary<string, string> { ["search"] = searchTerm });
}
