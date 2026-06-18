using AutomationFramework.Core.Exceptions;
using AutomationFramework.Core.Interfaces;

namespace AutomationFramework.Core.Drivers;

/// <summary>
/// Thread-safe ambient context that exposes the browser driver for the
/// current asynchronous execution context without requiring constructor injection
/// inside page objects and components.
///
/// Pattern: Ambient Context — provides a well-known access point while
/// keeping the driver instance scoped to the running test.
/// </summary>
public static class DriverContext
{
    private static readonly AsyncLocal<IBrowserDriver?> _driver = new();

    /// <summary>
    /// The browser driver for the current async context.
    /// Throws <see cref="DriverException"/> if not yet initialised.
    /// </summary>
    public static IBrowserDriver Current
    {
        get => _driver.Value
               ?? throw new DriverException(
                   "DriverContext has not been initialised for this test. " +
                   "Ensure BaseUITest.Setup() ran before accessing DriverContext.Current.");
        set => _driver.Value = value;
    }

    /// <summary>Returns true when a driver is bound to the current async context.</summary>
    public static bool IsInitialized => _driver.Value is not null;

    /// <summary>Clears the driver reference for the current async context.</summary>
    public static void Clear() => _driver.Value = null;
}
