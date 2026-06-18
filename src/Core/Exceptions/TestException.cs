namespace AutomationFramework.Core.Exceptions;

/// <summary>
/// Thrown when a test precondition is not met or a framework-level
/// test operation fails (distinct from assertion failures).
/// </summary>
public sealed class TestException : Exception
{
    public TestException(string message)
        : base(message) { }

    public TestException(string message, Exception innerException)
        : base(message, innerException) { }
}
