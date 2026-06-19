using AutomationFramework.Core.Constants;
using AutomationFramework.Core.Exceptions;
using AutomationFramework.Core.Interfaces;
using DotNetEnv;

namespace AutomationFramework.Core.Configuration;

/// <summary>
/// Singleton configuration manager that reads from environment-specific .env files.
/// Uses the Singleton Pattern to ensure one configuration source per process.
///
/// Load order (highest priority first):
///   1. Process environment variables (set by CI/CD or OS)
///   2. config/.env.{TEST_ENVIRONMENT} file
///   3. Hardcoded defaults
/// </summary>
public sealed class AppConfigurationManager : IConfigurationManager
{
    private static readonly Lazy<AppConfigurationManager> _instance =
        new(() => new AppConfigurationManager(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static AppConfigurationManager Instance => _instance.Value;

    private AppConfigurationManager()
    {
        LoadEnvironmentFile();
    }

    // ── Public interface ──────────────────────────────────────────────────────

    public string GetEnvironment() =>
        Environment.GetEnvironmentVariable(TestConstants.EnvKeyEnvironment)
        ?? Environment.GetEnvironmentVariable("ENVIRONMENT")
        ?? "dev";

    public string GetBaseUrl() => GetRequired(TestConstants.EnvKeyBaseUrl);

    public string GetApiUrl() => GetRequired(TestConstants.EnvKeyApiUrl);

    public string GetBrowser() =>
        Environment.GetEnvironmentVariable(TestConstants.EnvKeyBrowser) ?? BrowserConstants.Chromium;

    public bool IsHeadless()
    {
        var raw = Environment.GetEnvironmentVariable(TestConstants.EnvKeyHeadless) ?? "true";
        return bool.TryParse(raw, out var result) ? result : true;
    }

    public int GetTimeout()
    {
        var raw = Environment.GetEnvironmentVariable(TestConstants.EnvKeyTimeout)
                  ?? TestConstants.DefaultTimeout.ToString();
        return int.TryParse(raw, out var result) ? result : TestConstants.DefaultTimeout;
    }

    public string Get(string key, string defaultValue = "") =>
        Environment.GetEnvironmentVariable(key) ?? defaultValue;

    // ── Private helpers ───────────────────────────────────────────────────────

    private void LoadEnvironmentFile()
    {
        var env = GetEnvironment();
        var envFilePath = ResolveEnvFilePath(env);

        // clobberExistingVars: false ensures process-level env vars (e.g. HEADLESS, BROWSER
        // set via $env:HEADLESS="false" or CI/CD) always take priority over .env file values.
        var loadOptions = new LoadOptions(clobberExistingVars: false);

        if (File.Exists(envFilePath))
        {
            Env.Load(envFilePath, loadOptions);
        }
        else
        {
            // Fallback to .env.dev so local runs always work
            var devPath = ResolveEnvFilePath("dev");
            if (File.Exists(devPath))
                Env.Load(devPath, loadOptions);
        }
    }

    private static string ResolveEnvFilePath(string environment)
    {
        var root = FindSolutionRoot();
        return Path.Combine(root, "config", $".env.{environment.ToLower()}");
    }

    /// <summary>
    /// Walks up from the current working directory until it finds the
    /// solution root (identified by AutomationFramework.sln).
    /// </summary>
    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    private static string GetRequired(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new ConfigurationException(
                $"Required configuration key '{key}' is missing. " +
                $"Set it in your .env file or as a process environment variable.");
        return value;
    }
}
