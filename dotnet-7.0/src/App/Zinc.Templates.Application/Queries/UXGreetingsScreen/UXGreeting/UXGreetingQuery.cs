using System;
using RedLine.Application.Queries;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Queries.UXGreetingsScreen.UXGreeting
{
    /// <summary>
    /// A ux query.
    /// </summary>
    public class UXGreetingQuery : ResourceQueryBase<Greeting, UXGreetingResult>
    {
        /// <summary>
        /// Initialize the query.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="greetingId">The greeting id.</param>
        public UXGreetingQuery(string tenantId, Guid correlationId, Guid greetingId)
            : base(tenantId, correlationId, greetingId.ToString())
        {
            GreetingId = greetingId;
        }

        /// <summary>
        /// The greeting id.
        /// </summary>
        public Guid GreetingId { get; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Finds greetings to display on the greeting screen.";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Query greeting";
    }
}
