using AutomationFramework.Core.Logging;
using AutomationFramework.UI.Workflows;
using Microsoft.Playwright;

namespace AutomationFramework.Business.UIFlows;

/// <summary>
/// Orchestrates the project creation end-to-end UI flow.
/// </summary>
public sealed class ProjectCreationFlow
{
    private readonly IPage _page;
    private readonly NavigationWorkflow _navigation;

    private const string ProjectNameInput  = "[data-testid='project-name'], input[name='projectName'], #project-name";
    private const string ProjectDescInput  = "[data-testid='project-desc'], textarea[name='description'], #project-desc";
    private const string CreateProjectBtn  = "[data-testid='create-project'], button:has-text('Create Project'), #create-project";
    private const string SuccessToast     = "[data-testid='success-toast'], .toast-success, .alert-success";

    public ProjectCreationFlow(IPage page, string baseUrl)
    {
        _page      = page;
        _navigation = new NavigationWorkflow(page, baseUrl);
    }

    /// <summary>
    /// Navigates to the projects page, fills the form, and submits.
    /// Returns true if the success message appears.
    /// </summary>
    public async Task<bool> CreateProjectAsync(string projectName, string description)
    {
        LoggerService.Instance.Information("Creating project via UI: {ProjectName}", projectName);

        await _navigation.GoToProjectsAsync();
        await _page.ClickAsync("[data-testid='new-project-btn'], button:has-text('New Project')");
        await _page.FillAsync(ProjectNameInput, projectName);
        await _page.FillAsync(ProjectDescInput, description);
        await _page.ClickAsync(CreateProjectBtn);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        try
        {
            await _page.WaitForSelectorAsync(SuccessToast,
                new PageWaitForSelectorOptions { Timeout = 5_000 });
            LoggerService.Instance.Information("Project created successfully: {ProjectName}", projectName);
            return true;
        }
        catch
        {
            LoggerService.Instance.Warning("Project creation did not show success toast for: {ProjectName}", projectName);
            return false;
        }
    }
}
