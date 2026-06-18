# AutomationFramework

A production-ready, enterprise-scale test automation framework built with **C# (.NET 10)**, **Playwright**, **NUnit**, **RestSharp**, **FluentAssertions**, **ExtentReports**, and **Serilog**.

Supports UI automation, REST API testing, parallel execution, multi-environment configuration, rich HTML reports, and full CI/CD integration.

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Technology Stack](#technology-stack)
3. [Project Structure](#project-structure)
4. [Prerequisites](#prerequisites)
5. [Installation](#installation)
6. [Configuration & Environments](#configuration--environments)
7. [Running Tests](#running-tests)
8. [Browser Selection](#browser-selection)
9. [Parallel Execution](#parallel-execution)
10. [Reports](#reports)
11. [Logging](#logging)
12. [Adding New Tests](#adding-new-tests)
13. [CI/CD Integration](#cicd-integration)
14. [Design Patterns](#design-patterns)
15. [Troubleshooting](#troubleshooting)

---

## Architecture Overview

```
Tests (UITests / APITests)
       │
       ▼
  Business Layer  ←───────────────────────┐
  (Flows / Facades)                        │
       │                                   │
  ┌────▼────┐    ┌───────────────┐         │
  │  UI     │    │  API          │         │
  │  Layer  │    │  Layer        │         │
  └────┬────┘    └───────┬───────┘         │
       │                 │                 │
       └────────┬────────┘                 │
                ▼                          │
          Core Layer                       │
  (Config / Drivers / Interfaces)          │
                │                          │
          Infrastructure  ────────────────┘
  (Reporting / Screenshots / DI)
```

---

## Technology Stack

| Concern            | Technology                        |
|--------------------|-----------------------------------|
| Language           | C# 13 / .NET 10                   |
| UI Automation      | Microsoft.Playwright              |
| API Automation     | RestSharp                         |
| Test Runner        | NUnit 4                           |
| Assertions         | FluentAssertions 6                |
| Reporting          | AventStack.ExtentReports 5 / Allure |
| Logging            | Serilog (Console + File)          |
| Configuration      | DotNetEnv                         |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |
| Data Serialisation | Newtonsoft.Json                   |
| CSV Parsing        | CsvHelper                         |

---

## Project Structure

```
AutomationFramework/
├── src/
│   ├── Core/               # Framework foundation
│   │   ├── Configuration/  # AppConfigurationManager, EnvironmentConfig
│   │   ├── Constants/      # BrowserConstants, TestConstants
│   │   ├── Drivers/        # BrowserFactory, BrowserManager, DriverContext
│   │   ├── Enums/          # BrowserType, EnvironmentType
│   │   ├── Exceptions/     # ConfigurationException, DriverException
│   │   ├── Extensions/     # PageExtensions, StringExtensions
│   │   ├── Helpers/        # DateTimeHelper, RetryHelper
│   │   ├── Interfaces/     # IBrowserDriver, IConfigurationManager, ITestReporter
│   │   ├── Logging/        # LoggerService (Serilog)
│   │   └── Utilities/      # TestDataReader, JsonDataReader, CsvDataReader
│   │
│   ├── UI/                 # Page Object Model
│   │   ├── Pages/          # BasePage, LoginPage, DashboardPage, UsersPage
│   │   ├── Components/     # HeaderComponent, MenuComponent, GridComponent, ModalComponent
│   │   ├── Locators/       # LoginLocators, DashboardLocators, UsersLocators
│   │   └── Workflows/      # NavigationWorkflow
│   │
│   ├── API/                # REST API layer
│   │   ├── Clients/        # BaseApiClient, UsersApiClient, ProjectsApiClient
│   │   ├── Endpoints/      # UserEndpoints, ProjectEndpoints
│   │   ├── Requests/       # CreateUserRequest, UpdateUserRequest, etc.
│   │   ├── Responses/      # UserResponse, ProjectResponse, ApiResponse
│   │   └── Services/       # UserService, ProjectService
│   │
│   ├── Business/           # Business logic layer
│   │   ├── UIFlows/        # LoginFlow, UserManagementFlow, ProjectCreationFlow
│   │   ├── APIFacades/     # UserApiFacade, ProjectApiFacade
│   │   └── DomainServices/ # AuthenticationService
│   │
│   └── Infrastructure/     # Cross-cutting concerns
│       ├── Reporting/      # ExtentReportManager, AllureReportManager
│       ├── Screenshots/    # ScreenshotManager
│       └── DependencyInjection/  # ServiceCollectionExtensions
│
├── tests/
│   ├── UITests/
│   │   ├── Base/           # GlobalSetup, BaseUITest
│   │   ├── Smoke/          # LoginSmokeTests
│   │   ├── Regression/     # UserManagementTests
│   │   └── E2E/            # ProjectCreationE2ETests
│   │
│   ├── APITests/
│   │   ├── Base/           # ApiGlobalSetup, BaseApiTest
│   │   ├── Smoke/          # UserApiSmokeTests
│   │   ├── Regression/     # UserApiRegressionTests
│   │   └── Integration/    # UserProjectIntegrationTests
│   │
│   └── TestData/
│       ├── Json/           # users.json, projects.json
│       ├── Csv/            # test_users.csv
│       └── Payloads/       # create_user_payload.json
│
├── config/                 # .env.dev, .env.qa, .env.stage, .env.prod
├── reports/                # Generated HTML reports (git-ignored)
├── logs/                   # Daily log files (git-ignored)
├── screenshots/            # Failure screenshots (git-ignored)
├── docs/                   # ARCHITECTURE.md
├── .github/workflows/      # GitHub Actions CI/CD
├── azure-pipelines.yml     # Azure DevOps pipeline
└── .gitlab-ci.yml          # GitLab CI pipeline
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Node.js (required by Playwright to download browsers)
- Git

---

## Installation

```bash
# 1. Clone the repository
git clone https://github.com/your-org/AutomationFramework.git
cd AutomationFramework

# 2. Restore NuGet packages
dotnet restore AutomationFramework.sln

# 3. Build the solution
dotnet build AutomationFramework.sln

# 4. Install Playwright browsers
pwsh tests/UITests/bin/Release/net10.0/playwright.ps1 install
# Or: playwright install --with-deps

# 5. Copy and configure your environment file
cp config/.env.dev config/.env.local
# Edit config/.env.dev with your BASE_URL, API_URL, USERNAME, PASSWORD
```

---

## Configuration & Environments

Environment files live in `config/` and are loaded automatically:

| File              | Usage                    |
|-------------------|--------------------------|
| `config/.env.dev` | Local development        |
| `config/.env.qa`  | QA/Testing environment   |
| `config/.env.stage` | Staging environment    |
| `config/.env.prod`  | Production smoke tests |

### Switching environments

```bash
# Via environment variable (highest priority)
TEST_ENVIRONMENT=qa dotnet test

# Via CI/CD environment variable
dotnet test  # TEST_ENVIRONMENT is set by the pipeline
```

### Example `.env` file

```env
BASE_URL=https://qa.yourapp.com
API_URL=https://api-qa.yourapp.com
USERNAME=testuser@qa.yourapp.com
PASSWORD=SecurePass123!
BROWSER=chromium
HEADLESS=true
DEFAULT_TIMEOUT=30000
TEST_ENVIRONMENT=qa
```

---

## Running Tests

### Run all tests
```bash
dotnet test AutomationFramework.sln
```

### Run only API tests
```bash
dotnet test tests/APITests/APITests.csproj
```

### Run only UI tests
```bash
dotnet test tests/UITests/UITests.csproj
```

### Filter by category
```bash
# Run Smoke tests only
dotnet test --filter "Category=Smoke"

# Run Regression tests only
dotnet test --filter "Category=Regression"

# Run API Smoke tests
dotnet test tests/APITests/APITests.csproj --filter "Category=Smoke&Category=API"

# Run E2E UI tests
dotnet test tests/UITests/UITests.csproj --filter "Category=E2E"
```

### With NUnit runsettings
```bash
dotnet test --settings nunit.runsettings
```

---

## Browser Selection

```bash
# Chromium (default)
BROWSER=chromium dotnet test tests/UITests/UITests.csproj

# Google Chrome (requires Chrome to be installed)
BROWSER=chrome dotnet test tests/UITests/UITests.csproj

# Mozilla Firefox
BROWSER=firefox dotnet test tests/UITests/UITests.csproj

# Microsoft Edge
BROWSER=edge dotnet test tests/UITests/UITests.csproj

# WebKit (Safari engine)
BROWSER=webkit dotnet test tests/UITests/UITests.csproj

# Headed mode (visible browser window)
HEADLESS=false BROWSER=chromium dotnet test tests/UITests/UITests.csproj
```

---

## Parallel Execution

Tests are decorated with `[Parallelizable(ParallelScope.Fixtures)]`, meaning different test fixture classes run concurrently. Each test gets its own browser instance via `AsyncLocal<IBrowserDriver>`.

Control the worker count via `nunit.runsettings`:
```xml
<NUnit>
  <Workers>4</Workers>
</NUnit>
```

Or at the command line:
```bash
dotnet test -- NUnit.Workers=8
```

---

## Reports

### ExtentReports (HTML)
Reports are automatically generated after each test run:
```
reports/TestReport_yyyyMMdd_HHmmss.html
```

Open in any browser. Includes:
- Pass/Fail/Skip summary with charts
- Per-test step logs
- Inline failure screenshots
- Environment and browser information

### Allure Reports
To use Allure instead of ExtentReports, swap the DI registration in `ServiceCollectionExtensions.cs`:
```csharp
services.AddAutomationFramework(useAllureReporter: true);
```

Then generate the report:
```bash
allure serve allure-results
```

---

## Logging

Logs are written to `logs/{date}.log` and the console:

```
logs/
  20260618.log
  20260619.log
```

Log levels: `Debug → Information → Warning → Error → Fatal`

Every request/response, browser launch, test start/end, and screenshot capture is logged.

---

## Adding New Tests

### New UI Test
1. Create a new file in `tests/UITests/Regression/` (or `Smoke/` / `E2E/`)
2. Inherit from `BaseUITest`
3. Use Business Flows (not pages directly):

```csharp
[TestFixture]
[Category("Regression")]
public class MyNewTests : BaseUITest
{
    private LoginFlow _login = null!;

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _login = new LoginFlow(Page, BaseUrl);
        await _login.LoginAsync("user@example.com", "password");
    }

    [Test]
    public async Task MyTest_DoesSomething_ExpectedResult()
    {
        // arrange, act, assert
    }
}
```

### New API Test
1. Create a file in `tests/APITests/Regression/`
2. Inherit from `BaseApiTest`
3. Use Facades:

```csharp
[TestFixture]
[Category("Regression")]
public class MyApiTests : BaseApiTest
{
    private UserApiFacade _users = null!;

    [SetUp]
    public new void SetUp() { base.SetUp(); _users = new UserApiFacade(ApiBaseUrl); }

    [Test]
    public async Task GetUser_ReturnsValidData()
    {
        var user = await _users.GetUserByIdAsync(1);
        user.Should().NotBeNull();
    }
}
```

### New Page Object
1. Create `src/UI/Locators/MyPageLocators.cs` for selectors
2. Create `src/UI/Pages/MyPage.cs` inheriting from `BasePage`
3. Create `src/Business/UIFlows/MyFlow.cs` for business logic

---

## CI/CD Integration

| Platform       | File                         |
|----------------|------------------------------|
| GitHub Actions | `.github/workflows/ci.yml`   |
| Azure DevOps   | `azure-pipelines.yml`        |
| GitLab CI      | `.gitlab-ci.yml`             |

### Required Secrets/Variables

| Secret              | Description                  |
|---------------------|------------------------------|
| `BASE_URL_QA`       | QA environment URL           |
| `API_URL_QA`        | QA API URL                   |
| `TEST_USERNAME_QA`  | QA test username             |
| `TEST_PASSWORD_QA`  | QA test password             |

---

## Design Patterns

| Pattern        | Where Used                              | Why                                                   |
|----------------|-----------------------------------------|-------------------------------------------------------|
| **Factory**    | `BrowserFactory`                        | Creates browser drivers without exposing Playwright   |
| **Singleton**  | `AppConfigurationManager`, `LoggerService` | One config/logger instance per process            |
| **Strategy**   | `ITestReporter` (Extent vs Allure)      | Swap reporters without changing test code             |
| **Facade**     | `UserApiFacade`, `LoginFlow`            | Simplify multi-step operations for test authors       |
| **Template Method** | `BaseApiClient`, `BasePage`        | Shared mechanics; subclasses fill in specifics        |
| **Ambient Context** | `DriverContext`                    | Zero-arg page access inside page objects              |
| **Builder**    | `ServiceCollectionExtensions`           | Fluent DI registration                                |
| **Repository** | `UserService`, `ProjectService`         | Encapsulates data access patterns                     |

---

## Troubleshooting

### "Required configuration key 'BASE_URL' is missing"
Ensure you have a `config/.env.dev` (or the relevant env file) with `BASE_URL=...` set.

### Playwright browser not found
```bash
pwsh tests/UITests/bin/Release/net10.0/playwright.ps1 install
# or
playwright install --with-deps
```

### Tests failing in CI/CD with timeout
Increase `DEFAULT_TIMEOUT` in the env file or `.env` secrets, e.g., `DEFAULT_TIMEOUT=60000`.

### Screenshots not generated
Check that the `screenshots/` directory is writable and that the test base class's `TearDown` is being called.

### Parallel tests conflicting
Ensure each test class uses only its own browser instance via `DriverContext`. Never share `IPage` or `IBrowserDriver` across fixtures.
