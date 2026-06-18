using AutomationFramework.API.Clients;
using AutomationFramework.API.Requests;
using AutomationFramework.API.Responses;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.API.Services;

/// <summary>
/// Business-level service for Project operations.
/// </summary>
public sealed class ProjectService : IDisposable
{
    private readonly ProjectsApiClient _client;

    public ProjectService(string apiBaseUrl, int timeoutMs = 30_000)
    {
        _client = new ProjectsApiClient(apiBaseUrl, timeoutMs);
    }

    public async Task<IList<ProjectResponse>> GetAllProjectsAsync()
    {
        LoggerService.Instance.Information("Fetching all projects");
        var response = await _client.GetAllProjectsAsync();
        return response.Data ?? [];
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(int id)
    {
        LoggerService.Instance.Information("Fetching project with ID: {Id}", id);
        var response = await _client.GetProjectByIdAsync(id);
        return response.IsSuccessful ? response.Data : null;
    }

    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
    {
        LoggerService.Instance.Information("Creating project: {Title}", request.Title);
        var response = await _client.CreateProjectAsync(request);

        if (!response.IsSuccessful)
            throw new InvalidOperationException(
                $"Failed to create project. Status: {(int)response.StatusCode}");

        return response.Data!;
    }

    public async Task<ProjectResponse> UpdateProjectAsync(int id, UpdateProjectRequest request)
    {
        LoggerService.Instance.Information("Updating project ID: {Id}", id);
        var response = await _client.UpdateProjectAsync(id, request);

        if (!response.IsSuccessful)
            throw new InvalidOperationException(
                $"Failed to update project {id}. Status: {(int)response.StatusCode}");

        return response.Data!;
    }

    public async Task DeleteProjectAsync(int id)
    {
        LoggerService.Instance.Information("Deleting project ID: {Id}", id);
        var response = await _client.DeleteProjectAsync(id);

        if (!response.IsSuccessful)
            throw new InvalidOperationException(
                $"Failed to delete project {id}. Status: {(int)response.StatusCode}");
    }

    public async Task<IList<ProjectResponse>> GetProjectsByUserIdAsync(int userId)
    {
        var all = await GetAllProjectsAsync();
        return all.Where(p => p.UserId == userId).ToList();
    }

    public void Dispose() => _client.Dispose();
}
