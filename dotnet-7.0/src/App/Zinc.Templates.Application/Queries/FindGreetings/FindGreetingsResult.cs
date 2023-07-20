using System;

namespace Zinc.Templates.Application.Queries.FindGreetings
{
    /// <summary>
    /// Sample result.
    /// </summary>
    public record FindGreetingsResult
    {
        /// <summary>
        /// Initializes a new instance of the record.
        /// </summary>
        /// <param name="greetingId">The greeting unique identifier.</param>
        /// <param name="message">The greeting message.</param>
        /// <param name="tenantId">Tenant id.</param>
        public FindGreetingsResult(Guid greetingId, string message, string tenantId)
        {
            GreetingId = greetingId;
            Message = message;
            TenantId = tenantId;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FindGreetingsResult()
        {
        }

        /// <summary>
        /// Gets or sets the greeting unique identifier.
        /// </summary>
        public Guid GreetingId { get; init; }

        /// <summary>
        /// Gets or sets the greeting message.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Gets or sets the tenant id.
        /// </summary>
        public string TenantId { get; init; }
    }
}
