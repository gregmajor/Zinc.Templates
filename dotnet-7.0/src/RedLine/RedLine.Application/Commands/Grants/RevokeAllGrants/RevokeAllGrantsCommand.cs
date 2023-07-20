using System;

namespace RedLine.Application.Commands.Grants.RevokeAllGrants
{
    /// <summary>
    /// The command used to revoke a grant from a user.
    /// </summary>
    public class RevokeAllGrantsCommand : CommandBase<AuditableResult<bool>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevokeAllGrantsCommand"/>.
        /// </summary>
        /// <param name="tenantId">The tenant unique identifier.</param>
        /// <param name="correlationId">A unique, client-specified correlation identifier for the request.</param>
        /// <param name="userId">The user to which the grant will be bestowed.</param>
        /// <param name="fullName">The human-readable user full name.</param>
        public RevokeAllGrantsCommand(
            string tenantId,
            Guid correlationId,
            string userId,
            string fullName)
            : base(tenantId, correlationId)
        {
            UserId = userId;
            FullName = fullName;
        }

        /// <summary>
        /// The user from which the grant will be revoked.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// The human-readable user full name.
        /// </summary>
        public string FullName { get; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Revokes all grants from a user for a tenant.";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Revoke all grants";
    }
}
