namespace AutomationFramework.Core.Interfaces;

/// <summary>
/// Abstraction over the test reporting mechanism.
/// Allows swapping ExtentReports for Allure (or any other reporter)
/// without changing test code — Strategy Pattern.
/// </summary>
public interface ITestReporter
{
    /// <summary>Creates a new test node in the report.</summary>
    void StartTest(string testName, string description = "");

    /// <summary>Logs an informational step (supports structured format args).</summary>
    void LogInfo(string messageTemplate, params object?[] args);

    /// <summary>Logs a warning step (supports structured format args).</summary>
    void LogWarning(string messageTemplate, params object?[] args);

    /// <summary>Logs an error step with optional exception details.</summary>
    void LogError(string message, Exception? exception = null);

    /// <summary>Attaches a screenshot file to the current test node.</summary>
    void AttachScreenshot(string screenshotPath, string title = "Screenshot");

    /// <summary>Marks the current test as passed.</summary>
    void PassTest(string? message = null);

    /// <summary>Marks the current test as failed.</summary>
    void FailTest(string reason, Exception? exception = null);

    /// <summary>Marks the current test as skipped.</summary>
    void SkipTest(string reason);

    /// <summary>Finalises the current test node.</summary>
    void EndTest();

    /// <summary>Flushes and writes the HTML report to disk.</summary>
    void GenerateReport();
}
