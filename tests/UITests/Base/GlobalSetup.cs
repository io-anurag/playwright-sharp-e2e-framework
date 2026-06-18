using AutomationFramework.Core.Configuration;
using AutomationFramework.Core.Logging;
using AutomationFramework.Infrastructure.Reporting;
using NUnit.Framework;

namespace AutomationFramework.UITests.Base;

/// <summary>
/// Assembly-level one-time setup and teardown.
/// Initialises the ExtentReport before the first test and flushes it after the last.
/// NUnit's [SetUpFixture] scoped to the root namespace covers the entire assembly.
/// </summary>
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        LoggerService.Instance.Information("═══════════════════════════════════════════════════════════");
        LoggerService.Instance.Information("  TEST RUN STARTED");
        LoggerService.Instance.Information("  Environment : {Env}", AppConfigurationManager.Instance.GetEnvironment());
        LoggerService.Instance.Information("  Browser     : {Browser}", AppConfigurationManager.Instance.GetBrowser());
        LoggerService.Instance.Information("  Base URL    : {Url}", AppConfigurationManager.Instance.GetBaseUrl());
        LoggerService.Instance.Information("═══════════════════════════════════════════════════════════");

        ExtentReportManager.InitialiseReport();
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        ExtentReportManager.Flush();
        LoggerService.Instance.Information("TEST RUN FINISHED. Report generated.");
    }
}
