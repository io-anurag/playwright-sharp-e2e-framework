using AutomationFramework.API.Responses;
using AutomationFramework.APITests;
using AutomationFramework.Business.APIFacades;
using FluentAssertions;
using NUnit.Framework;
using System.Net;

namespace AutomationFramework.APITests.Smoke;

/// <summary>
/// Smoke tests that verify the Users API is up and returning valid responses.
/// These run first in CI/CD to gate more expensive regression tests.
/// </summary>
[TestFixture]
[Category("Smoke")]
[Category("API")]
public sealed class UserApiSmokeTests : BaseApiTest
{
    private UserApiFacade _userFacade = null!;

    [SetUp]
    public new void SetUp()
    {
        base.SetUp();
        _userFacade = new UserApiFacade(ApiBaseUrl);
    }

    [TearDown]
    public new void TearDown()
    {
        _userFacade.Dispose();
        base.TearDown();
    }

    [Test]
    [Description("GET /users returns HTTP 200 and a non-empty list.")]
    public async Task GetAllUsers_Returns200_AndNonEmptyList()
    {
        Reporter.LogInfo("Verifying GET /users endpoint");

        var users = await _userFacade.GetAllUsersAsync();

        users.Should().NotBeNullOrEmpty(ValidationMessages.ApiUsers.NonEmpty);
        users.Should().AllSatisfy(u =>
        {
            u.Id.Should().BeGreaterThan(0);
            u.Name.Should().NotBeNullOrWhiteSpace();
            u.Email.Should().Contain("@");
        });

        Reporter.LogInfo("GET /users returned {Count} users", users.Count);
    }

    [Test]
    [Description("GET /users/1 returns a user with ID 1.")]
    public async Task GetUserById_WithValidId_ReturnsCorrectUser()
    {
        Reporter.LogInfo("Verifying GET /users/1");

        var user = await _userFacade.GetUserByIdAsync(1);

        user.Should().NotBeNull(ValidationMessages.ApiUsers.ByIdExists);
        user!.Id.Should().Be(1);
        user.Name.Should().NotBeNullOrWhiteSpace();

        Reporter.LogInfo("User retrieved: {Name} ({Email})", user.Name, user.Email);
    }

    [Test]
    [Description("POST /users creates a new user and returns HTTP 201 with the created object.")]
    public async Task CreateUser_WithValidPayload_Returns201()
    {
        Reporter.LogInfo("Verifying POST /users");

        var created = await _userFacade.CreateUserAsync(
            name: "Smoke Test User",
            username: $"smokeuser{DateTime.Now.Ticks}",
            email: $"smoke{DateTime.Now.Ticks}@test.com");

        created.Should().NotBeNull();
        created.Id.Should().BeGreaterThan(0, ValidationMessages.ApiUsers.CreateIdAssigned);
        created.Name.Should().Be("Smoke Test User");

        Reporter.LogInfo("User created with ID: {Id}", created.Id);
    }
}
