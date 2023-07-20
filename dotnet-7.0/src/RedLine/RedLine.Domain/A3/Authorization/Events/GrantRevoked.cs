using System;

namespace RedLine.A3.Authorization.Events
{
    /// <summary>
    /// The event that is raised when a grant is revoked from a user.
    /// </summary>
    public class GrantRevoked : IGrantRevoked
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="fullName">The full name of the grantee.</param>
        /// <param name="applicationName">The application in which the grant was bestowed.</param>
        /// <param name="tenantId">The tenant for which the grant was bestowed.</param>
        /// <param name="grantType">The type of grant bestowed.</param>
        /// <param name="qualifier">The qualifier of the grant bestowed.</param>
        /// <param name="expiresOn">The grant expiration date, or null.</param>
        /// <param name="revokedBy">The user who bestowed the grant.</param>
        /// <param name="revokedOn">The date/time the grant was bestowed.</param>
        public GrantRevoked(
            string userId,
            string fullName,
            string applicationName,
            string tenantId,
            string grantType,
            string qualifier,
            DateTimeOffset? expiresOn,
            string revokedBy,
            DateTimeOffset revokedOn)
        {
            UserId = userId;
            FullName = fullName;
            ApplicationName = applicationName;
            TenantId = tenantId;
            GrantType = grantType;
            Qualifier = qualifier;
            ExpiresOn = expiresOn;
            RevokedBy = revokedBy;
            RevokedOn = revokedOn;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GrantRevoked()
        { }

        /// <summary>
        /// Gets the application name in which the grant was revoked.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the date/time when the grant expires, or null if it does not expire.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// Gets the full name of the grantee.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets the type of grant that was revoked.
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets the qualifier for the grant that was revoked.
        /// </summary>
        public string Qualifier { get; set; }

        /// <summary>
        /// Gets the user who revoked the grant.
        /// </summary>
        public string RevokedBy { get; set; }

        /// <summary>
        /// Gets the date/time when the grant was revoked.
        /// </summary>
        public DateTimeOffset RevokedOn { get; set; }

        /// <summary>
        /// Gets the tenant for which the grant was revoked.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets the user id of the grantee.
        /// </summary>
        public string UserId { get; set; }
    }
}
