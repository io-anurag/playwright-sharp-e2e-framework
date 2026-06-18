using Allure.Net.Commons;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Infrastructure.Reporting;

/// <summary>
/// Allure-based test reporter.
/// Use this instead of <see cref="ExtentReportManager"/> by swapping the
/// DI registration in <c>ServiceCollectionExtensions</c>.
///
/// Allure writes JSON results to the allure-results/ directory.
/// Run `allure serve allure-results` to view the HTML report.
///
/// Note: Allure NUnit integration also works through assembly-level attributes:
///   [assembly: AllureSuite("Smoke Tests")]
/// and per-test: [AllureFeature("Login")], [AllureStory("Valid Credentials")]
/// </summary>
public sealed class AllureReportManager : ITestReporter
{
    private readonly AllureLifecycle _lifecycle = AllureLifecycle.Instance;
    private string _currentTestUuid = string.Empty;

    public void StartTest(string testName, string description = "")
    {
        _currentTestUuid = Guid.NewGuid().ToString();
        LoggerService.Instance.Information("Allure: Starting test '{TestName}'", testName);
    }

    public void LogInfo(string messageTemplate, params object?[] args)
    {
        LoggerService.Instance.Information(messageTemplate, args);
        _lifecycle.UpdateTestCase(tc =>
            tc.steps.Add(new StepResult { name = messageTemplate, status = Status.passed }));
    }

    public void LogWarning(string messageTemplate, params object?[] args)
    {
        LoggerService.Instance.Warning(messageTemplate, args);
    }

    public void LogError(string message, Exception? exception = null)
    {
        LoggerService.Instance.Error(message);
        _lifecycle.UpdateTestCase(tc =>
        {
            tc.status = Status.failed;
            tc.statusDetails = new StatusDetails
            {
                message = message,
                trace   = exception?.StackTrace
            };
        });
    }

    public void AttachScreenshot(string screenshotPath, string title = "Screenshot")
    {
        if (!File.Exists(screenshotPath)) return;
        // AllureApi.AddAttachment is the correct surface in Allure.Net.Commons 2.12+
        AllureApi.AddAttachment(title, "image/png", screenshotPath);
    }

    public void PassTest(string? message = null)
    {
        _lifecycle.UpdateTestCase(tc => tc.status = Status.passed);
    }

    public void FailTest(string reason, Exception? exception = null)
    {
        _lifecycle.UpdateTestCase(tc =>
        {
            tc.status = Status.failed;
            tc.statusDetails = new StatusDetails
            {
                message = reason,
                trace   = exception?.StackTrace
            };
        });
    }

    public void SkipTest(string reason)
    {
        _lifecycle.UpdateTestCase(tc =>
        {
            tc.status = Status.skipped;
            tc.statusDetails = new StatusDetails { message = reason };
        });
    }

    public void EndTest()
    {
        _lifecycle.StopTestCase();
        _lifecycle.WriteTestCase();
    }

    public void GenerateReport()
    {
        LoggerService.Instance.Information(
            "Allure results written to allure-results/. " +
            "Run 'allure serve allure-results' to view the HTML report.");
    }
}
