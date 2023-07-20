using System;

namespace Zinc.Templates.Application.Queries.GetGreeting
{
    /// <summary>
    /// Result for <see cref="GetGreetingQuery" />.
    /// </summary>
    public record GetGreetingResult
    {
        /// <summary>
        /// Initializes a new instance of the record.
        /// </summary>
        /// <param name="greetingId">The greeting unique identifier.</param>
        /// <param name="message">The greeting message.</param>
        /// <param name="tenantId">Tenant id.</param>
        public GetGreetingResult(Guid greetingId, string message, string tenantId)
        {
            GreetingId = greetingId;
            Message = message;
            TenantId = tenantId;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GetGreetingResult()
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
