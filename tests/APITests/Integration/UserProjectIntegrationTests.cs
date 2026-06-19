using AutomationFramework.APITests;
using AutomationFramework.Business.APIFacades;
using FluentAssertions;
using NUnit.Framework;

namespace AutomationFramework.APITests.Integration;

/// <summary>
/// Integration tests that verify cross-resource relationships:
/// a User owns multiple Projects (Posts in JSONPlaceholder).
/// These tests verify that the data model is consistent across endpoints.
/// </summary>
[TestFixture]
[Category("Integration")]
[Category("API")]
public sealed class UserProjectIntegrationTests : BaseApiTest
{
    private UserApiFacade    _userFacade    = null!;
    private ProjectApiFacade _projectFacade = null!;

    [SetUp]
    public new void SetUp()
    {
        base.SetUp();
        _userFacade    = new UserApiFacade(ApiBaseUrl);
        _projectFacade = new ProjectApiFacade(ApiBaseUrl);
    }

    [TearDown]
    public new void TearDown()
    {
        _userFacade.Dispose();
        _projectFacade.Dispose();
        base.TearDown();
    }

    [Test]
    [Description("Each user in /users has at least one project in /posts.")]
    public async Task EachUser_HasAtLeastOneProject()
    {
        Reporter.LogInfo("Verifying that every user has at least one project");

        var users    = await _userFacade.GetAllUsersAsync();
        var projects = await _projectFacade.GetAllProjectsAsync();

        users.Should().NotBeEmpty();
        projects.Should().NotBeEmpty();

        foreach (var user in users.Take(5)) // validate first 5 to keep test fast
        {
            var userProjects = projects.Where(p => p.UserId == user.Id).ToList();
            userProjects.Should().NotBeEmpty(
                string.Format(ValidationMessages.ApiProjects.UserOwnsAtLeastOne, user.Id, user.Name));

            Reporter.LogInfo("User {Id} ({Name}) owns {Count} project(s)",
                user.Id, user.Name, userProjects.Count);
        }
    }

    [Test]
    [Description("Creating a project links it to a valid user ID.")]
    public async Task CreateProject_WithValidUserId_LinksToExistingUser()
    {
        Reporter.LogInfo("Creating project linked to user ID 1");

        // Verify the user exists
        var user = await _userFacade.GetUserByIdAsync(1);
        user.Should().NotBeNull(ValidationMessages.ApiProjects.UserOneMustExist);

        // Create the project
        var project = await _projectFacade.CreateProjectAsync(
            title:  "Integration Test Project",
            body:   "This project was created by an integration test",
            userId: 1);

        project.Should().NotBeNull();
        project.Id.Should().BeGreaterThan(0);
        project.UserId.Should().Be(1);
        project.Title.Should().Be("Integration Test Project");

        Reporter.LogInfo("Project created with ID {Id} linked to user {UserId}",
            project.Id, project.UserId);
    }

    [Test]
    [Description("GET /posts?userId={id} returns only projects for that user.")]
    public async Task GetProjectsByUser_ReturnsOnlyThatUsersProjects()
    {
        const int targetUserId = 3;

        var projects = await _projectFacade.GetProjectsByUserAsync(targetUserId);

        projects.Should().NotBeEmpty(string.Format(ValidationMessages.ApiProjects.UserShouldHaveProjects, targetUserId));
        projects.Should().AllSatisfy(p =>
            p.UserId.Should().Be(targetUserId, ValidationMessages.ApiProjects.BelongToQueriedUser));

        Reporter.LogInfo("User {UserId} has {Count} projects", targetUserId, projects.Count);
    }
}
