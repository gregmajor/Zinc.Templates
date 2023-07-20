using System;

namespace RedLine.Domain.Model
{
    /// <summary>
    /// Allows for abstracting calls such as DateTimeOffset.UtcNow (particularly in unit tests).
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        /// <returns>The current date and time.</returns>
        DateTimeOffset Now();
    }
}