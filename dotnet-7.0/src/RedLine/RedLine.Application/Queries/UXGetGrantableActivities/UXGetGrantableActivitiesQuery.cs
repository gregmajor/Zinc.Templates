using System;
using System.Collections.Generic;

namespace RedLine.Application.Queries.UXGetGrantableActivities
{
    /// <summary>
    /// Queries for grantable activities by user.
    /// </summary>
    public class UXGetGrantableActivitiesQuery : QueryBase<IEnumerable<UXGrantableActivity>>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="userId">The user id.</param>
        public UXGetGrantableActivitiesQuery(string tenantId, Guid correlationId, string userId)
            : base(tenantId, correlationId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Gets the user id for grants.
        /// </summary>
        public string UserId { get; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Gets the activities which a user has or could have granted.";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Get grantable activities";
    }
}
