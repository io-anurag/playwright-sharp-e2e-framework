using AutomationFramework.Core.Configuration;
using AutomationFramework.Core.Logging;
using AutomationFramework.Infrastructure.Reporting;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Utilities;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AutomationFramework.APITests;

/// <summary>
/// Assembly-level one-time setup for the API test suite.
/// </summary>
[SetUpFixture]
public class ApiGlobalSetup
{
    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        LoggerService.Instance.Information("API TEST RUN STARTED | Environment: {Env} | API: {ApiUrl}",
            AppConfigurationManager.Instance.GetEnvironment(),
            AppConfigurationManager.Instance.GetApiUrl());
        ExtentReportManager.InitialiseReport();
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        ExtentReportManager.Flush();
        LoggerService.Instance.Information("API TEST RUN FINISHED.");
    }
}

/// <summary>
/// Base class for all API tests.
///
/// Responsibilities:
///   • Provide quick access to <see cref="AppConfigurationManager"/>
///   • Start/end a report node for each test
///   • Log request/response details via Serilog
/// </summary>
[Parallelizable(ParallelScope.Fixtures)]
[TestFixture]
public abstract class BaseApiTest
{
    protected ITestReporter Reporter { get; private set; } = null!;
    protected TestDataReader TestData { get; private set; } = null!;

    protected string ApiBaseUrl => AppConfigurationManager.Instance.GetApiUrl();

    [SetUp]
    public void SetUp()
    {
        var testName = TestContext.CurrentContext.Test.FullName;
        LoggerService.Instance.Information("▶ API TEST STARTED: {TestName}", testName);

        Reporter = new ExtentReportManager();
        TestData = new TestDataReader();
        Reporter.StartTest(testName);
        Reporter.LogInfo($"API Base URL: {ApiBaseUrl}");
    }

    [TearDown]
    public void TearDown()
    {
        var result   = TestContext.CurrentContext.Result;
        var testName = TestContext.CurrentContext.Test.FullName;

        if (result.Outcome.Status == TestStatus.Failed)
        {
            LoggerService.Instance.Error("✖ API TEST FAILED: {TestName} — {Message}", testName, result.Message);
            Reporter.FailTest(result.Message ?? "Test failed");
        }
        else if (result.Outcome.Status == TestStatus.Skipped)
        {
            Reporter.SkipTest(result.Message ?? "Skipped");
        }
        else
        {
            LoggerService.Instance.Information("✔ API TEST PASSED: {TestName}", testName);
            Reporter.PassTest();
        }

        Reporter.EndTest();
    }
}
