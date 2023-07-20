using System;

namespace RedLine.Domain.Model
{
    /// <inheritdoc/>
    public class Clock : IClock
    {
        /// <inheritdoc/>
        public DateTimeOffset Now()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
