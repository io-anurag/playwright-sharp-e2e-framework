namespace AutomationFramework.Core.Configuration;

/// <summary>
/// Immutable snapshot of all resolved configuration values for one test run.
/// Created once by <see cref="AppConfigurationManager"/> and passed through DI.
/// Using a record ensures thread-safe, value-semantic configuration.
/// </summary>
public sealed record EnvironmentConfig(
    string Environment,
    string BaseUrl,
    string ApiUrl,
    string Username,
    string Password,
    string Browser,
    bool   Headless,
    int    Timeout
)
{
    /// <summary>
    /// Builds an <see cref="EnvironmentConfig"/> from the singleton
    /// <see cref="AppConfigurationManager"/>.
    /// </summary>
    public static EnvironmentConfig FromManager(AppConfigurationManager mgr) => new(
        Environment: mgr.GetEnvironment(),
        BaseUrl:     mgr.GetBaseUrl(),
        ApiUrl:      mgr.GetApiUrl(),
        Username:    mgr.Get("USERNAME"),
        Password:    mgr.Get("PASSWORD"),
        Browser:     mgr.GetBrowser(),
        Headless:    mgr.IsHeadless(),
        Timeout:     mgr.GetTimeout()
    );
}
