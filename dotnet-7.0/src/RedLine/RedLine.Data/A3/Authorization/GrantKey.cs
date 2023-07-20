using System;
using RedLine.A3.Authorization.Domain;

namespace RedLine.Data.A3.Authorization
{
    /// <summary>
    /// A helper class to manage the grant key used by the repository.
    /// </summary>
    public class GrantKey
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="grantType">The grant type.</param>
        /// <param name="qualifier">The grant policy.</param>
        public GrantKey(string userId, string tenantId, string grantType, string qualifier)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException($"The {nameof(userId)} argument is required.");
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentException($"The {nameof(tenantId)} argument is required.");
            }

            if (string.IsNullOrEmpty(grantType))
            {
                throw new ArgumentException($"The {nameof(grantType)} argument is required.");
            }

            if (string.IsNullOrEmpty(qualifier))
            {
                throw new ArgumentException($"The {nameof(qualifier)} argument is required.");
            }

            UserId = userId;
            Scope = new GrantScope(tenantId, grantType, qualifier);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="key">The string version of grant key, in the following format: {UserId}:{TenantId}:{GrantType}:{Qualifier}.</param>
        public GrantKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"The {nameof(key)} argument is required.");
            }

            var parts = key.Split(':', 4, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 4)
            {
                throw new ArgumentException($"The {nameof(key)} argument is invalid. The expected format is: {{userId}}:{{tenant}}:{{type}}:{{qualifier}}.");
            }

            UserId = parts[0];
            Scope = new GrantScope(parts[1], parts[2], parts[3]);
        }

        /// <summary>
        /// The user identifier.
        /// </summary>
        public string UserId { get; init; }

        /// <summary>
        /// The grant scope.
        /// </summary>
        public GrantScope Scope { get; init; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{UserId}:{Scope}";
        }
    }
}
