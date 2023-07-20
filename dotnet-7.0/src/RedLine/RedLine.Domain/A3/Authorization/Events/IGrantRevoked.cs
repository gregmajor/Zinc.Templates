using System;
using RedLine.Domain.Events;

namespace RedLine.A3.Authorization.Events
{
    /// <summary>
    /// The event that is raised when a grant is revoked from a user.
    /// </summary>
    public interface IGrantRevoked : IDomainEvent
    {
        /// <summary>
        /// Gets the application name in which the grant was revoked.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets the date/time when the grant expires, or null if it does not expire.
        /// </summary>
        DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// Gets the full name of the grantee.
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// Gets the type of grant that was revoked.
        /// </summary>
        string GrantType { get; set; }

        /// <summary>
        /// Gets the qualifier for the grant that was revoked.
        /// </summary>
        string Qualifier { get; set; }

        /// <summary>
        /// Gets the user who revoked the grant.
        /// </summary>
        string RevokedBy { get; set; }

        /// <summary>
        /// Gets the date/time when the grant was revoked.
        /// </summary>
        DateTimeOffset RevokedOn { get; set; }

        /// <summary>
        /// Gets the tenant for which the grant was revoked.
        /// </summary>
        string TenantId { get; set; }

        /// <summary>
        /// Gets the user id of the grantee.
        /// </summary>
        string UserId { get; set; }
    }
}
