using AutomationFramework.API.Endpoints;
using AutomationFramework.API.Requests;
using AutomationFramework.API.Responses;
using RestSharp;

namespace AutomationFramework.API.Clients;

/// <summary>
/// API client for the Projects resource.
/// </summary>
public sealed class ProjectsApiClient : BaseApiClient
{
    public ProjectsApiClient(string baseUrl, int timeoutMs = 30_000)
        : base(baseUrl, timeoutMs) { }

    public async Task<RestResponse<List<ProjectResponse>>> GetAllProjectsAsync() =>
        await GetAsync<List<ProjectResponse>>(ProjectEndpoints.GetAll);

    public async Task<RestResponse<ProjectResponse>> GetProjectByIdAsync(int id) =>
        await GetAsync<ProjectResponse>(ProjectEndpoints.GetById(id));

    public async Task<RestResponse<ProjectResponse>> CreateProjectAsync(CreateProjectRequest request) =>
        await PostAsync<ProjectResponse>(ProjectEndpoints.Create, request);

    public async Task<RestResponse<ProjectResponse>> UpdateProjectAsync(int id, UpdateProjectRequest request) =>
        await PutAsync<ProjectResponse>(ProjectEndpoints.Update(id), request);

    public async Task<RestResponse> DeleteProjectAsync(int id) =>
        await DeleteAsync(ProjectEndpoints.Delete(id));
}
