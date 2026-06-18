namespace AutomationFramework.Core.Interfaces;

/// <summary>
/// Contract for reading and resolving framework configuration.
/// Implements the Strategy Pattern — concrete implementations can target
/// .env files, appsettings, Azure Key Vault, etc.
/// </summary>
public interface IConfigurationManager
{
    /// <summary>Returns the active environment name (dev/qa/stage/prod).</summary>
    string GetEnvironment();

    /// <summary>Returns the base URL of the web application under test.</summary>
    string GetBaseUrl();

    /// <summary>Returns the base URL of the REST API under test.</summary>
    string GetApiUrl();

    /// <summary>Returns the browser to use (chromium/chrome/firefox/edge/webkit).</summary>
    string GetBrowser();

    /// <summary>Returns whether the browser should run headless.</summary>
    bool IsHeadless();

    /// <summary>Returns the global timeout in milliseconds.</summary>
    int GetTimeout();

    /// <summary>Gets an arbitrary environment variable with an optional fallback.</summary>
    string Get(string key, string defaultValue = "");
}
