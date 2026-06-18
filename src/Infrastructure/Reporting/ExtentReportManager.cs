using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using AutomationFramework.Core.Configuration;
using AutomationFramework.Core.Constants;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Logging;

namespace AutomationFramework.Infrastructure.Reporting;

/// <summary>
/// Manages ExtentReports for the entire test run.
/// Uses AsyncLocal so each parallel test maintains its own ExtentTest node
/// without race conditions.
///
/// Lifecycle:
///   1. Call <see cref="InitialiseReport"/> once in GlobalSetup.
///   2. Call <see cref="CreateTest"/> in each test's SetUp.
///   3. Call <see cref="Flush"/> once in GlobalTeardown.
/// </summary>
public sealed class ExtentReportManager : ITestReporter
{
    private static ExtentReports? _extent;
    private static readonly AsyncLocal<ExtentTest?> _currentTest = new();
    private static readonly Lock _lock = new();

    // ── Singleton-style initialisation (called from GlobalSetup) ─────────────

    public static void InitialiseReport()
    {
        lock (_lock)
        {
            if (_extent is not null) return;

            var root       = FindSolutionRoot();
            var reportsDir = Path.Combine(root, TestConstants.ReportsDirectory);
            Directory.CreateDirectory(reportsDir);

            var reportPath = Path.Combine(
                reportsDir,
                $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");

            var sparkReporter = new ExtentSparkReporter(reportPath);
            sparkReporter.Config.DocumentTitle = "Automation Test Report";
            sparkReporter.Config.ReportName    = "Test Execution Report";
            sparkReporter.Config.Theme         = Theme.Dark;

            _extent = new ExtentReports();
            _extent.AttachReporter(sparkReporter);
            _extent.AddSystemInfo("OS",          Environment.OSVersion.ToString());
            _extent.AddSystemInfo("Machine",     Environment.MachineName);
            _extent.AddSystemInfo(".NET",        Environment.Version.ToString());
            _extent.AddSystemInfo("Environment", AppConfigurationManager.Instance.GetEnvironment());
            _extent.AddSystemInfo("Browser",     AppConfigurationManager.Instance.GetBrowser());

            LoggerService.Instance.Information("ExtentReport initialised at: {Path}", reportPath);
        }
    }

    public static void Flush()
    {
        lock (_lock)
        {
            _extent?.Flush();
            LoggerService.Instance.Information("ExtentReport flushed.");
        }
    }

    // ── ITestReporter implementation ──────────────────────────────────────────

    public void StartTest(string testName, string description = "")
    {
        if (_extent is null) InitialiseReport();
        _currentTest.Value = _extent!.CreateTest(testName, description);
    }

    public void LogInfo(string messageTemplate, params object?[] args)
    {
        _currentTest.Value?.Info(FormatMessage(messageTemplate, args));
        LoggerService.Instance.Information(messageTemplate, args);
    }

    public void LogWarning(string messageTemplate, params object?[] args)
    {
        _currentTest.Value?.Warning(FormatMessage(messageTemplate, args));
        LoggerService.Instance.Warning(messageTemplate, args);
    }

    public void LogError(string message, Exception? exception = null)
    {
        var fullMessage = exception is not null
            ? $"{message}\n{exception.Message}\n{exception.StackTrace}"
            : message;
        _currentTest.Value?.Fail(fullMessage);
        LoggerService.Instance.Error(message);
    }

    public void AttachScreenshot(string screenshotPath, string title = "Screenshot")
    {
        if (File.Exists(screenshotPath))
            _currentTest.Value?.AddScreenCaptureFromPath(screenshotPath, title);
    }

    public void PassTest(string? message = null)
    {
        if (message is not null)
            _currentTest.Value?.Pass(message);
        else
            _currentTest.Value?.Pass("Test passed");
    }

    public void FailTest(string reason, Exception? exception = null)
    {
        var message = exception is not null
            ? $"{reason}\n{exception}"
            : reason;
        _currentTest.Value?.Fail(message);
    }

    public void SkipTest(string reason)
    {
        _currentTest.Value?.Skip(reason);
    }

    public void EndTest()
    {
        // ExtentReports auto-finalises tests on flush — no explicit close needed
    }

    public void GenerateReport() => Flush();

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Replaces Serilog-style named tokens ({Name}, {Count}, etc.) positionally
    /// with their runtime values so ExtentReports shows readable messages.
    /// </summary>
    private static string FormatMessage(string template, object?[] args)
    {
        if (args.Length == 0) return template;
        int index = 0;
        return System.Text.RegularExpressions.Regex.Replace(
            template,
            @"\{[^}]+\}",
            _ => index < args.Length ? (args[index++]?.ToString() ?? "null") : "?");
    }

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0) return dir.FullName;
            dir = dir.Parent;
        }
        return AppDomain.CurrentDomain.BaseDirectory;
    }
}
