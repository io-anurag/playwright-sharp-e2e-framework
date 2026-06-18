namespace AutomationFramework.Core.Constants;

/// <summary>
/// Framework-wide test constants.
/// Centralises magic numbers and string values used across the codebase.
/// </summary>
public static class TestConstants
{
    // ── Timeouts (milliseconds) ───────────────────────────────────────────────
    public const int DefaultTimeout        = 30_000;
    public const int ShortTimeout          = 5_000;
    public const int LongTimeout           = 60_000;
    public const int NavigationTimeout     = 30_000;
    public const int ApiTimeout            = 15_000;

    // ── Retry policy ─────────────────────────────────────────────────────────
    public const int DefaultRetryCount     = 3;
    public const int RetryDelayMs          = 1_000;

    // ── Directories ──────────────────────────────────────────────────────────
    public const string ReportsDirectory      = "reports";
    public const string ScreenshotsDirectory  = "screenshots";
    public const string LogsDirectory         = "logs";
    public const string TestDataDirectory     = "TestData";

    // ── Environment variable keys ─────────────────────────────────────────────
    public const string EnvKeyEnvironment  = "TEST_ENVIRONMENT";
    public const string EnvKeyBrowser      = "BROWSER";
    public const string EnvKeyHeadless     = "HEADLESS";
    public const string EnvKeyBaseUrl      = "BASE_URL";
    public const string EnvKeyApiUrl       = "API_URL";
    public const string EnvKeyUsername     = "USERNAME";
    public const string EnvKeyPassword     = "PASSWORD";
    public const string EnvKeyTimeout      = "DEFAULT_TIMEOUT";
}
