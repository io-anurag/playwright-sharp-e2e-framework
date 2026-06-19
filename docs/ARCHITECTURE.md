# Architecture Documentation

## Overview

The playwright-sharp-e2e-framework follows **Clean Architecture** principles, separating concerns into concentric layers. Inner layers know nothing about outer layers; dependencies only point inward.

```
┌─────────────────────────────────────────────────────────┐
│                        Tests                            │
│              UITests          APITests                  │
├────────────────────────┬────────────────────────────────┤
│        Business Layer  │                                │
│   UIFlows  APIFacades  DomainServices                  │
├──────────────┬─────────┴────────────────────────────────┤
│    UI Layer  │           API Layer                      │
│  Pages       │  Clients  Endpoints                      │
│  Components  │  Models   Services                       │
│  Locators    │  Requests/Responses                      │
├──────────────┴─────────────────────────────────────────-┤
│                     Core Layer                          │
│  Configuration  Drivers  Interfaces  Enums  Exceptions  │
│  Extensions     Helpers  Logging     Utilities          │
├─────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                   │
│    ExtentReportManager  AllureReportManager             │
│    ScreenshotManager    DependencyInjection             │
└─────────────────────────────────────────────────────────┘
```

---

## Layer Responsibilities

### Core Layer (`src/Core`)
The innermost layer. Contains zero business logic and zero test-specific code.

| Component               | Responsibility                                                        |
|-------------------------|-----------------------------------------------------------------------|
| `IConfigurationManager` | Contract for reading config — env files, appsettings, etc.           |
| `IBrowserDriver`        | Abstraction over any browser automation library                       |
| `ITestReporter`         | Strategy contract for swapping reporting engines                      |
| `AppConfigurationManager` | Singleton config reader backed by DotNetEnv                        |
| `BrowserFactory`        | Creates `IBrowserDriver` instances (Factory Pattern)                  |
| `BrowserManager`        | Manages browser lifecycle per async context (AsyncLocal)              |
| `DriverContext`         | Ambient context exposing current driver to page objects               |
| `LoggerService`         | Singleton Serilog wrapper                                             |
| `RetryHelper`           | Configurable retry logic for flaky operations                         |
| `TestDataReader`        | Facade over JSON and CSV test data sources                            |

### UI Layer (`src/UI`)
Implements the **Page Object Model (POM)**. Pages are pure interaction libraries — no assertions, no business flow knowledge.

| Component          | Responsibility                                                  |
|--------------------|-----------------------------------------------------------------|
| `BasePage`         | Common page interactions (click, fill, getText, waitFor...)     |
| `LoginPage`        | Login page interactions only                                    |
| `DashboardPage`    | Dashboard interactions only                                     |
| `UsersPage`        | Users table/form interactions                                   |
| `*Locators`        | CSS/XPath selectors isolated from page action code              |
| `BaseComponent`    | Common component interactions                                   |
| `GridComponent`    | Reusable data table interaction (pagination, sorting, reading)  |
| `ModalComponent`   | Reusable modal/dialog interaction                               |
| `NavigationWorkflow` | URL-based navigation primitives                               |

### API Layer (`src/API`)
Mirrors the POM pattern for REST APIs. Clients are thin wrappers; services add domain logic.

| Component       | Responsibility                                           |
|-----------------|----------------------------------------------------------|
| `BaseApiClient` | HTTP mechanics (RestSharp), request/response logging     |
| `UsersApiClient`| Typed methods for each `/users` endpoint                 |
| `UserEndpoints` | Centralised endpoint URL constants                       |
| `CreateUserRequest` | API request payload model                           |
| `UserResponse`  | API response model                                       |
| `UserService`   | Business-level operations (validation, orchestration)    |

### Business Layer (`src/Business`)
Orchestrates multi-page or multi-API flows. Tests call business flows, not pages or clients directly. This is where "do a login and assert dashboard" lives.

| Component           | Responsibility                                         |
|---------------------|--------------------------------------------------------|
| `LoginFlow`         | Full login flow: navigate → fill → submit → verify     |
| `UserManagementFlow`| Create/search/delete users via UI                      |
| `UserApiFacade`     | High-level API actions: create, find, delete users     |
| `AuthenticationService` | Token management (stub — replace with real impl) |

### Infrastructure Layer (`src/Infrastructure`)
Implements cross-cutting concerns. Depends on Core interfaces but never on UI/API/Business.

| Component              | Responsibility                                        |
|------------------------|-------------------------------------------------------|
| `ExtentReportManager`  | Implements `ITestReporter` using ExtentReports 5      |
| `AllureReportManager`  | Implements `ITestReporter` using Allure.NUnit         |
| `ScreenshotManager`    | Captures and stores screenshots with safe file naming |
| `ServiceCollectionExtensions` | Registers all services in the DI container   |

---

## Design Patterns Reference

### Factory Pattern — `BrowserFactory`
```
Client code (BaseUITest)
     │ calls CreateAsync("chromium")
     ▼
BrowserFactory
     │ creates PlaywrightBrowserDriver (wraps IPlaywright + IBrowser)
     ▼
IBrowserDriver  ←── returned to caller; Playwright is hidden
```
**Benefit:** Adding Selenium requires only a new `SeleniumBrowserDriver : IBrowserDriver` and one extra branch in `BrowserFactory`. No test code changes.

### Singleton Pattern — `AppConfigurationManager`, `LoggerService`
Both use `Lazy<T>` with `ExecutionAndPublication` thread safety. The single instance is shared across all parallel tests, which is safe because both are read-only after initialisation.

### Strategy Pattern — `ITestReporter`
```csharp
// Register the preferred reporter in DI:
services.AddSingleton<ITestReporter, ExtentReportManager>();
// or
services.AddSingleton<ITestReporter, AllureReportManager>();
```
`BaseUITest` and `BaseApiTest` receive `ITestReporter` — they never reference ExtentReports or Allure directly.

### Facade Pattern — `LoginFlow`, `UserApiFacade`
```
Test code: await _loginFlow.LoginAsync(user, pass)
                │
                ▼ (hides:)
    NavigationWorkflow.GoToLoginAsync()
    LoginPage.EnterUsernameAsync()
    LoginPage.EnterPasswordAsync()
    LoginPage.ClickLoginButtonAsync()
    DashboardPage.IsDashboardLoadedAsync()
```

### Ambient Context Pattern — `DriverContext`
```csharp
// In BaseUITest.SetUp:
DriverContext.Current = browserDriver;

// Anywhere inside a page object:
var page = DriverContext.Current.Page;
```
Avoids constructor parameter pollution when page objects are nested deeply.

### Template Method Pattern — `BasePage`, `BaseApiClient`
Base classes define the algorithm skeleton (how to click, how to send HTTP); subclasses provide the specifics (which locators, which endpoints).

---

## Dependency Graph

```
UITests ──┐
APITests ──┼──► Business ──► UI  ──┐
            │              API ──┼──► Core ◄── Infrastructure
            └──────────────────────┘
```

- All arrows point **inward** (toward Core)
- Core has **no project references** — only NuGet packages
- Infrastructure implements Core interfaces but is not referenced by UI/API/Business

---

## Extensibility Guide

### Adding a new browser
1. Add to `BrowserConstants` (new string constant)
2. Add to `BrowserType` enum
3. Add a case in `BrowserFactory.CreateAsync`

### Adding a new page
1. Create `src/UI/Locators/MyPageLocators.cs`
2. Create `src/UI/Pages/MyPage.cs : BasePage`
3. Optionally create `src/Business/UIFlows/MyFlow.cs`
4. Write tests in `tests/UITests/`

### Adding a new API resource
1. Create `src/API/Endpoints/MyEndpoints.cs`
2. Create request/response models in `src/API/Requests/` and `Responses/`
3. Create `src/API/Clients/MyApiClient.cs : BaseApiClient`
4. Create `src/API/Services/MyService.cs`
5. Optionally create `src/Business/APIFacades/MyFacade.cs`
6. Write tests in `tests/APITests/`

### Switching from ExtentReports to Allure
Change one line in `ServiceCollectionExtensions.cs`:
```csharp
// Before:
services.AddSingleton<ITestReporter, ExtentReportManager>();
// After:
services.AddSingleton<ITestReporter, AllureReportManager>();
```
All test code continues to compile and run unchanged.

### Adding a new environment
1. Create `config/.env.{envname}`
2. Set `TEST_ENVIRONMENT={envname}` before running tests
3. Add the environment to CI/CD matrix if required

---

## Thread Safety Considerations

| Concern                | Mechanism                              |
|------------------------|----------------------------------------|
| Browser isolation      | `AsyncLocal<IBrowserDriver>` per test  |
| Report node isolation  | `AsyncLocal<ExtentTest>` per test      |
| Config reads           | Singleton, read-only after init        |
| Logger writes          | Serilog's thread-safe sinks            |
| Screenshot naming      | Timestamp + test name (unique per run) |

---

## Performance Recommendations

- Run Smoke tests in every PR (~30 s)
- Run Regression tests on merge to main (~5 min)
- Run E2E tests on schedule or before release (~15 min)
- Use `ParallelScope.Fixtures` for independent test classes
- Keep UI tests focused: use API setup/teardown for preconditions where possible
- Pool browser instances across tests in the same fixture if tests are read-only
