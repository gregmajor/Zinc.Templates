using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;

namespace Zinc.Templates.Application.Queries.FindGreetings
{
    /// <summary>
    /// Searches for greetings matching a pattern.
    /// </summary>
    public class FindGreetingsQuery : QueryBase<PageableResult<FindGreetingsResult>>
    {
        /// <summary>
        /// Initialize the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="pattern">Find greetings containing pattern.</param>
        public FindGreetingsQuery(string tenantId, Guid correlationId, string pattern)
            : base(tenantId, correlationId)
        {
            Pattern = pattern;
        }

        /// <summary>
        /// Message to search by.
        /// </summary>
        public string Pattern { get; init; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Searches for greetings that match your criteria.";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Find greetings";
    }
}
