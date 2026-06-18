using AutomationFramework.API.Clients;
using AutomationFramework.API.Requests;
using AutomationFramework.APITests;
using AutomationFramework.Business.APIFacades;
using FluentAssertions;
using NUnit.Framework;

namespace AutomationFramework.APITests.Regression;

/// <summary>
/// Full CRUD regression suite for the Users API.
/// Uses JSONPlaceholder (https://jsonplaceholder.typicode.com) as the API under test.
/// </summary>
[TestFixture]
[Category("Regression")]
[Category("API")]
[Category("Users")]
public sealed class UserApiRegressionTests : BaseApiTest
{
    private UserApiFacade    _userFacade  = null!;
    private UsersApiClient   _rawClient   = null!;

    [SetUp]
    public new void SetUp()
    {
        base.SetUp();
        _userFacade = new UserApiFacade(ApiBaseUrl);
        _rawClient  = new UsersApiClient(ApiBaseUrl);
    }

    [TearDown]
    public new void TearDown()
    {
        _userFacade.Dispose();
        _rawClient.Dispose();
        base.TearDown();
    }

    // ── GET ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("GET /users returns a list with exactly 10 users (JSONPlaceholder).")]
    public async Task GetAllUsers_ReturnsExpectedCount()
    {
        var users = await _userFacade.GetAllUsersAsync();
        users.Should().HaveCount(10, "JSONPlaceholder returns exactly 10 seed users");
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    [Description("GET /users/{id} returns the correct user for each valid ID.")]
    public async Task GetUserById_WithValidIds_ReturnsCorrectData(int userId)
    {
        Reporter.LogInfo("Fetching user with ID: {Id}", userId);

        var user = await _userFacade.GetUserByIdAsync(userId);

        user.Should().NotBeNull();
        user!.Id.Should().Be(userId);
        user.Email.Should().Contain("@");
        Reporter.LogInfo("Verified user: {Name}", user.Name);
    }

    [Test]
    [Description("GET /users/999 for a non-existent user returns a 404 response.")]
    public async Task GetUserById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _rawClient.GetUserByIdAsync(999);
        ((int)response.StatusCode).Should().Be(404,
            "a non-existent resource should return 404");
    }

    // ── POST ───────────────────────────────────────────────────────────────────

    [Test]
    [Description("POST /users with valid payload returns 201 and the created user.")]
    public async Task CreateUser_WithValidPayload_IsSuccessful()
    {
        var request = new CreateUserRequest
        {
            Name     = "Automation Test User",
            Username = $"autotest_{Guid.NewGuid():N}",
            Email    = $"auto_{Guid.NewGuid():N}@example.com",
            Phone    = "+1-555-0100",
            Website  = "automation.example.com"
        };

        var response = await _rawClient.CreateUserAsync(request);

        response.IsSuccessful.Should().BeTrue("POST /users should succeed");
        ((int)response.StatusCode).Should().Be(201, "resource creation must return 201");
        response.Data.Should().NotBeNull();
        response.Data!.Name.Should().Be(request.Name);
        response.Data.Email.Should().Be(request.Email);
    }

    // ── PUT ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("PUT /users/1 with valid payload updates the user.")]
    public async Task UpdateUser_WithValidPayload_IsSuccessful()
    {
        var request = new UpdateUserRequest
        {
            Name     = "Updated Name",
            Username = "updateduser",
            Email    = "updated@example.com"
        };

        var response = await _rawClient.UpdateUserAsync(1, request);

        response.IsSuccessful.Should().BeTrue("PUT /users/1 should succeed");
        response.Data!.Name.Should().Be("Updated Name");
    }

    // ── DELETE ─────────────────────────────────────────────────────────────────

    [Test]
    [Description("DELETE /users/1 returns 200 OK.")]
    public async Task DeleteUser_WithValidId_ReturnsOk()
    {
        var response = await _rawClient.DeleteUserAsync(1);
        response.IsSuccessful.Should().BeTrue("DELETE /users/1 should return 200");
    }

    // ── Schema validation ──────────────────────────────────────────────────────

    [Test]
    [Description("All users in GET /users have non-empty required fields.")]
    public async Task AllUsers_HaveRequiredFields_Populated()
    {
        var users = await _userFacade.GetAllUsersAsync();

        users.Should().AllSatisfy(user =>
        {
            user.Id.Should().BeGreaterThan(0, $"User ID should be positive");
            user.Name.Should().NotBeNullOrWhiteSpace($"User.Name should not be empty");
            user.Email.Should().NotBeNullOrWhiteSpace($"User.Email should not be empty");
            user.Email.Should().Contain("@", $"User.Email '{user.Email}' should be a valid email");
            user.Username.Should().NotBeNullOrWhiteSpace($"User.Username should not be empty");
        });
    }
}
