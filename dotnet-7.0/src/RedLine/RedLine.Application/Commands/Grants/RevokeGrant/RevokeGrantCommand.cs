using System;

namespace RedLine.Application.Commands.Grants.RevokeGrant
{
    /// <summary>
    /// Command to revoke a grant from a user.
    /// </summary>
    public class RevokeGrantCommand : CommandBase<AuditableResult<bool>>
    {
        /// <summary>
        /// Initializes the command.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="grantType">The type of the grant.</param>
        /// <param name="qualifier">The qualifier for the grant.</param>
        public RevokeGrantCommand(
            string tenantId,
            Guid correlationId,
            string userId,
            string grantType,
            string qualifier)
            : base(tenantId, correlationId)
        {
            UserId = userId;
            GrantType = grantType;
            Qualifier = qualifier;
        }

        /// <summary>
        /// The user id of the grantee.
        /// </summary>
        public string UserId { get; init; }

        /// <summary>
        /// The type of grant.
        /// </summary>
        public string GrantType { get; init; }

        /// <summary>
        /// The grant qualifier.
        /// </summary>
        public string Qualifier { get; init; }

        /// <inheritdoc />
        public override string ActivityDescription => "Revokes a user's permissions.";

        /// <inheritdoc />
        public override string ActivityDisplayName => "Revoke grant";
    }
}
