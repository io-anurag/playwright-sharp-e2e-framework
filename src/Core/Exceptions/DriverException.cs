namespace AutomationFramework.Core.Exceptions;

/// <summary>
/// Thrown when the browser driver cannot be initialised or a driver
/// operation fails (e.g., page not found, unsupported browser type).
/// </summary>
public sealed class DriverException : Exception
{
    public DriverException(string message)
        : base(message) { }

    public DriverException(string message, Exception innerException)
        : base(message, innerException) { }
}
