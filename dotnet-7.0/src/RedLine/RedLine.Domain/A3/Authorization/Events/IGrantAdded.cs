using System;
using RedLine.Domain.Events;

namespace RedLine.A3.Authorization.Events
{
    /// <summary>
    /// The event that is raised when a grant is added for a user.
    /// </summary>
    public interface IGrantAdded : IDomainEvent
    {
        /// <summary>
        /// Gets the application name in which the grant was added.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets the date/time when the grant expires, or null if it does not expire.
        /// </summary>
        DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// The full name of the grantee.
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// Gets the user who bestowed the grant.
        /// </summary>
        string GrantedBy { get; set; }

        /// <summary>
        /// Gets the date/time when the grant was bestowed.
        /// </summary>
        DateTimeOffset GrantedOn { get; set; }

        /// <summary>
        /// Gets the type of grant that was bestowed.
        /// </summary>
        string GrantType { get; set; }

        /// <summary>
        /// Gets the qualifier for the grant that was bestowed.
        /// </summary>
        string Qualifier { get; set; }

        /// <summary>
        /// Gets the tenant for which the grant was bestowed.
        /// </summary>
        string TenantId { get; set; }

        /// <summary>
        /// Gets the user id of the grantee.
        /// </summary>
        string UserId { get; set; }
    }
}
