using AutomationFramework.API.Requests;
using AutomationFramework.API.Responses;
using AutomationFramework.API.Services;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Business.APIFacades;

/// <summary>
/// Facade that exposes high-level project API operations to tests.
/// </summary>
public sealed class ProjectApiFacade : IDisposable
{
    private readonly ProjectService _projectService;

    public ProjectApiFacade(string apiBaseUrl, int timeoutMs = 30_000)
    {
        _projectService = new ProjectService(apiBaseUrl, timeoutMs);
    }

    public async Task<IList<ProjectResponse>> GetAllProjectsAsync() =>
        await _projectService.GetAllProjectsAsync();

    public async Task<ProjectResponse?> GetProjectByIdAsync(int id) =>
        await _projectService.GetProjectByIdAsync(id);

    public async Task<ProjectResponse> CreateProjectAsync(string title, string body, int userId)
    {
        LoggerService.Instance.Information("API Facade: Creating project '{Title}' for user {UserId}", title, userId);
        var request = new CreateProjectRequest
        {
            Title  = title,
            Body   = body,
            UserId = userId
        };
        return await _projectService.CreateProjectAsync(request);
    }

    public async Task DeleteProjectAsync(int id) =>
        await _projectService.DeleteProjectAsync(id);

    public async Task<IList<ProjectResponse>> GetProjectsByUserAsync(int userId) =>
        await _projectService.GetProjectsByUserIdAsync(userId);

    public void Dispose() => _projectService.Dispose();
}
