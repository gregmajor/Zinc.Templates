using System;
using RedLine.A3.Authorization.Events;
using RedLine.Domain;
using RedLine.Domain.Model;

namespace RedLine.A3.Authorization.Domain
{
    /// <summary>
    /// An aggregate that represents a grant for a user.
    /// </summary>
    public sealed class Grant : AggregateRootBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="fullName">The full name of the grantee.</param>
        /// <param name="tenantId">The tenant for which the grant is applicable.</param>
        /// <param name="grantType">The type of grant, typically Activity or ActivityGroup.</param>
        /// <param name="qualifier">The grant qualifier, such as the name of the activity or group.</param>
        /// <param name="expiresOn">The optional date and time at which the grant expires.</param>
        /// <param name="grantedBy">The admin user who is granting the permission.</param>
        public Grant(
            string userId,
            string fullName,
            string tenantId,
            string grantType,
            string qualifier,
            DateTimeOffset? expiresOn,
            string grantedBy)
            : this(userId, fullName, new GrantScope(tenantId, grantType, qualifier), expiresOn, grantedBy)
        { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="fullName">The full name of the grantee.</param>
        /// <param name="scope">The grant scope.</param>
        /// <param name="expiresOn">The optional date and time at which the grant expires.</param>
        /// <param name="grantedBy">The admin user who is granting the permission.</param>
        public Grant(
            string userId,
            string fullName,
            GrantScope scope,
            DateTimeOffset? expiresOn,
            string grantedBy)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException($"The {nameof(userId)} argument is required.");
            }

            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException($"The {nameof(fullName)} argument is required.");
            }

            if (scope == null)
            {
                throw new ArgumentException($"The {nameof(scope)} argument is required.");
            }

            if (expiresOn.HasValue && expiresOn.Value.ToUniversalTime() <= DateTimeOffset.UtcNow)
            {
                throw new ArgumentException($"The {nameof(expiresOn)} argument cannot be in the past.");
            }

            if (string.IsNullOrEmpty(grantedBy))
            {
                throw new ArgumentException($"The {nameof(grantedBy)} argument is required.");
            }

            UserId = userId;
            FullName = fullName;
            Scope = scope;
            ExpiresOn = expiresOn;
            GrantedBy = grantedBy;
            GrantedOn = DateTimeOffset.UtcNow;

            RaiseEvent(new GrantAdded(
                userId,
                fullName,
                ApplicationContext.ApplicationName,
                scope.TenantId,
                scope.GrantType,
                scope.Qualifier,
                expiresOn,
                grantedBy,
                GrantedOn));
        }

        /// <summary>
        /// This is an internal ctor and should **NOT** be made public.
        /// </summary>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="fullName">The full name of the grantee.</param>
        /// <param name="tenantId">The tenant for which the grant is applicable.</param>
        /// <param name="grantType">The type of grant, typically Activity or ActivityGroup.</param>
        /// <param name="qualifier">The grant qualifier, such as the name of the activity or group.</param>
        /// <param name="expiresOn">The optional date and time at which the grant expires.</param>
        /// <param name="grantedBy">The admin user who is granting the permission.</param>
        /// <param name="grantedOn">The date/time when the grant was added.</param>
        internal Grant(
            string userId,
            string fullName,
            string tenantId,
            string grantType,
            string qualifier,
            DateTimeOffset? expiresOn,
            string grantedBy,
            DateTimeOffset grantedOn)
        {
            UserId = userId;
            FullName = fullName;
            Scope = new GrantScope(tenantId, grantType, qualifier);
            ExpiresOn = expiresOn;
            GrantedBy = grantedBy;
            GrantedOn = grantedOn;
        }

        /// <summary>
        /// Gets the date and time at which the grant expires.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; internal set; }

        /// <summary>
        /// Gets the full name of the grantee.
        /// </summary>
        public string FullName { get; internal set; }

        /// <summary>
        /// Gets the user who bestowed the grant.
        /// </summary>
        public string GrantedBy { get; internal set; }

        /// <summary>
        /// Gets the date and time at which the grant was bestowed.
        /// </summary>
        public DateTimeOffset GrantedOn { get; internal set; }

        /// <inheritdoc />
        public override string Key => $"{UserId}:{Scope}";

        /// <summary>
        /// Gets the user who revoked the grant.
        /// </summary>
        public string RevokedBy { get; internal set; }

        /// <summary>
        /// Gets the date/time when the grant was revoked.
        /// </summary>
        public DateTimeOffset? RevokedOn { get; internal set; }

        /// <summary>
        /// Gets the grant scope in urn format.
        /// </summary>
        public GrantScope Scope { get; internal set; }

        /// <summary>
        /// Gets the user is of the grantee.
        /// </summary>
        public string UserId { get; internal set; }

        /// <summary>
        /// Determines if the grant is active - i.e. not expired and not revoked.
        /// </summary>
        /// <returns>True if the grant is active; otherwise, false.</returns>
        public bool IsActive() => !(IsExpired() || IsRevoked());

        /// <summary>
        /// Determines if the grant has expired.
        /// </summary>
        /// <returns>True if expired; otherwise, false.</returns>
        public bool IsExpired() => ExpiresOn.HasValue && ExpiresOn.Value <= DateTimeOffset.UtcNow;

        /// <summary>
        /// Determines if the grant has been revoked.
        /// </summary>
        /// <returns>True if the grant has been revoked; otherwise, false.</returns>
        public bool IsRevoked() => RevokedOn.HasValue;

        /// <summary>
        /// Revokes the grant.
        /// </summary>
        /// <param name="revokedBy">The user who is revoking the grant.</param>
        public void Revoke(string revokedBy)
        {
            RevokedBy = revokedBy;
            RevokedOn = DateTimeOffset.UtcNow;

            RaiseEvent(new GrantRevoked(
                UserId,
                FullName,
                ApplicationContext.ApplicationName,
                Scope.TenantId,
                Scope.GrantType,
                Scope.Qualifier,
                ExpiresOn,
                RevokedBy,
                RevokedOn.Value));
        }
    }
}
