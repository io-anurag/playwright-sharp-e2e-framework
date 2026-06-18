using AutomationFramework.Core.Configuration;
using AutomationFramework.Core.Drivers;
using AutomationFramework.Core.Interfaces;
using AutomationFramework.Core.Utilities;
using AutomationFramework.Infrastructure.Reporting;
using AutomationFramework.Infrastructure.Screenshots;
using Microsoft.Extensions.DependencyInjection;

namespace AutomationFramework.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering all framework services in the DI container.
///
/// Usage (in test setup or Program.cs):
/// <code>
/// var services = new ServiceCollection();
/// services.AddAutomationFramework();
/// var provider = services.BuildServiceProvider();
/// </code>
///
/// Design Pattern: Builder / Extension Method — keeps DI registration
/// out of test code and centralised for the entire framework.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all core, UI, API, and infrastructure services.
    /// </summary>
    public static IServiceCollection AddAutomationFramework(
        this IServiceCollection services,
        bool useAllureReporter = false)
    {
        // ── Configuration ─────────────────────────────────────────────────────
        services.AddSingleton<IConfigurationManager>(_ => AppConfigurationManager.Instance);
        services.AddSingleton(AppConfigurationManager.Instance);
        services.AddSingleton(sp =>
            EnvironmentConfig.FromManager(sp.GetRequiredService<AppConfigurationManager>()));

        // ── Browser management ────────────────────────────────────────────────
        services.AddSingleton<BrowserManager>();

        // ── Screenshot management ─────────────────────────────────────────────
        services.AddSingleton<ScreenshotManager>();

        // ── Reporting ─────────────────────────────────────────────────────────
        if (useAllureReporter)
            services.AddSingleton<ITestReporter, AllureReportManager>();
        else
            services.AddSingleton<ITestReporter, ExtentReportManager>();

        // ── Test data ─────────────────────────────────────────────────────────
        services.AddSingleton<TestDataReader>();

        return services;
    }
}
