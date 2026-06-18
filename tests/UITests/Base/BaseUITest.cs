using AutomationFramework.Core.Configuration;
using AutomationFramework.Core.Drivers;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;
using AutomationFramework.Core.Utilities;
using AutomationFramework.Infrastructure.Reporting;
using AutomationFramework.Infrastructure.Screenshots;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace AutomationFramework.UITests;

/// <summary>
/// Base class for all UI tests.
///
/// Responsibilities:
///   • Initialise a browser + page before each test ([SetUp])
///   • Capture a screenshot and attach it to the report on failure ([TearDown])
///   • Close the browser and finalise the report node after each test
///
/// Parallelism: set to NonParallelizable because all UI tests share the
/// same OrangeHRM demo account. Concurrent logins on the demo server
/// invalidate each other's sessions, causing false failures.
/// </summary>
[NonParallelizable]
[TestFixture]
public abstract class BaseUITest
{
    // ── Services (initialised in SetUp) ───────────────────────────────────────
    protected IBrowserDriver BrowserDriver { get; private set; } = null!;
    protected IPage          Page          { get; private set; } = null!;
    protected ITestReporter  Reporter      { get; private set; } = null!;
    protected TestDataReader TestData      { get; private set; } = null!;

    // ── Config shortcuts ──────────────────────────────────────────────────────
    protected string BaseUrl => AppConfigurationManager.Instance.GetBaseUrl();

    private readonly ScreenshotManager _screenshotManager = new();

    // ── NUnit lifecycle ───────────────────────────────────────────────────────

    [SetUp]
    public virtual async Task SetUp()
    {
        var testName = TestContext.CurrentContext.Test.FullName;
        LoggerService.Instance.Information("TEST STARTED: {TestName}", testName);

        // Create and register a fresh browser for this test
        BrowserDriver = await BrowserFactory.CreateAsync(
            AppConfigurationManager.Instance.GetBrowser(),
            AppConfigurationManager.Instance.IsHeadless());

        await BrowserDriver.InitializeAsync();

        // Publish to ambient context so page objects can access it without DI
        DriverContext.Current = BrowserDriver;

        Page     = BrowserDriver.Page;
        Reporter = new ExtentReportManager();
        TestData = new TestDataReader();

        Reporter.StartTest(testName);
        Reporter.LogInfo($"Browser: {AppConfigurationManager.Instance.GetBrowser()} | Headless: {AppConfigurationManager.Instance.IsHeadless()}");
    }

    [TearDown]
    public virtual async Task TearDown()
    {
        var result   = TestContext.CurrentContext.Result;
        var testName = TestContext.CurrentContext.Test.FullName;

        try
        {
            if (result.Outcome.Status == TestStatus.Failed)
            {
                LoggerService.Instance.Error("TEST FAILED: {TestName} — {Message}", testName, result.Message);

                // Capture screenshot and attach to report
                var screenshotPath = await _screenshotManager.CaptureOnFailureAsync(BrowserDriver, testName);
                if (screenshotPath is not null)
                    Reporter.AttachScreenshot(screenshotPath);

                Reporter.FailTest(result.Message ?? "Test failed", result.StackTrace is not null
                    ? new Exception(result.StackTrace)
                    : null);
            }
            else if (result.Outcome.Status == TestStatus.Skipped)
            {
                LoggerService.Instance.Warning("TEST SKIPPED: {TestName}", testName);
                Reporter.SkipTest(result.Message ?? "Test skipped");
            }
            else
            {
                LoggerService.Instance.Information("TEST PASSED: {TestName}", testName);
                Reporter.PassTest();
            }
        }
        finally
        {
            Reporter.EndTest();
            DriverContext.Clear();
            await BrowserDriver.DisposeAsync();
        }

        LoggerService.Instance.Information("TEST ENDED: {TestName}", testName);
    }
}
