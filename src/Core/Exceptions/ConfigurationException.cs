namespace AutomationFramework.Core.Exceptions;

/// <summary>
/// Thrown when a required configuration key is missing or invalid.
/// </summary>
public sealed class ConfigurationException : Exception
{
    public ConfigurationException(string message)
        : base(message) { }

    public ConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}
