namespace AutomationFramework.Core.Enums;

/// <summary>
/// Supported deployment environments.
/// Maps to the TEST_ENVIRONMENT variable and config/.env.{environment} files.
/// </summary>
public enum EnvironmentType
{
    Dev,
    Qa,
    Stage,
    Prod
}
