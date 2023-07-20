using System;
using RedLine.Application.Queries;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Queries.GetGreeting
{
    /// <summary>
    /// Sample Get query.
    /// </summary>
    public class GetGreetingQuery : ResourceQueryBase<Greeting, GetGreetingResult>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="id">The greeting id.</param>
        public GetGreetingQuery(string tenantId, Guid correlationId, Guid id)
            : base(tenantId, correlationId, id.ToString())
        {
            GreetingId = id;
        }

        /// <summary>
        /// Gets the greeting unique identifier.
        /// </summary>
        public Guid GreetingId { get; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Get a greeting model by id.";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Get greeting";
    }
}
