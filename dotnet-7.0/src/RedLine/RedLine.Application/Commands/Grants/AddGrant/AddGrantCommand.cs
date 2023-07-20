using System;

namespace RedLine.Application.Commands.Grants.AddGrant
{
    /// <summary>
    /// Command to create a grant for a user.
    /// </summary>
    public class AddGrantCommand : CommandBase<AuditableResult<bool>>
    {
        /// <summary>
        /// Initializes the command.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="fullName">The full name of the grantee.</param>
        /// <param name="grantType">The type of the grant.</param>
        /// <param name="qualifier">The qualifier for the grant.</param>
        /// <param name="expiresOn">Nullable expiration date.</param>
        public AddGrantCommand(
            string tenantId,
            Guid correlationId,
            string userId,
            string fullName,
            string grantType,
            string qualifier,
            DateTimeOffset? expiresOn)
            : base(tenantId, correlationId)
        {
            UserId = userId;
            FullName = fullName;
            GrantType = grantType;
            Qualifier = qualifier;
            ExpiresOn = expiresOn;
        }

        /// <summary>
        /// The user id of the grantee.
        /// </summary>
        public string UserId { get; init; }

        /// <summary>
        /// The full name of the grantee.
        /// </summary>
        public string FullName { get; init; }

        /// <summary>
        /// The type of grant.
        /// </summary>
        public string GrantType { get; init; }

        /// <summary>
        /// The grant qualifier.
        /// </summary>
        public string Qualifier { get; init; }

        /// <summary>
        /// When the grant expires.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; init; }

        /// <inheritdoc />
        public override string ActivityDescription => "Grants a user new permissions.";

        /// <inheritdoc />
        public override string ActivityDisplayName => "Add grant";
    }
}
