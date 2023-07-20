using System;

namespace Zinc.Templates.Application.Commands.PutGreeting
{
    /// <summary>
    /// The model used to upsert a greeting.
    /// </summary>
    public record PutGreetingModel
    {
        /// <summary>
        /// Gets or sets greeting unique identifier.
        /// </summary>
        public Guid GreetingId { get; init; }

        /// <summary>
        /// Gets or sets greeting message.
        /// </summary>
        public string Message { get; init; }
    }
}
