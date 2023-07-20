using System;

namespace RedLine.A3.Authorization.Domain
{
    /// <summary>
    /// The scope of a grant in the form [TenantId]:[GrantType]:[Qualifier].
    /// </summary>
    public record GrantScope
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="grantType">The type of grant.</param>
        /// <param name="qualifier">The grant qualifier.</param>
        public GrantScope(string tenantId, string grantType, string qualifier)
        {
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new ArgumentException($"The '{nameof(tenantId)}' argument is required.");
            }

            if (string.IsNullOrEmpty(grantType))
            {
                throw new ArgumentException($"The '{nameof(grantType)}' argument is required.");
            }

            if (string.IsNullOrEmpty(qualifier))
            {
                throw new ArgumentException($"The '{nameof(qualifier)}' argument is required.");
            }

            TenantId = tenantId;
            GrantType = grantType;
            Qualifier = qualifier;
        }

        /// <summary>
        /// Gets the tenant for the grant.
        /// </summary>
        public string TenantId { get; init; }

        /// <summary>
        /// Gets the grant type, typically 'Activity' or 'ActivityGroup', but could be anything.
        /// </summary>
        public string GrantType { get; init; }

        /// <summary>
        /// Gets the grant type qualifier, which is typically a name, an id, a path, etc., and which may include wildcards.
        /// </summary>
        public string Qualifier { get; init; }

        /// <summary>
        /// Initializes a scope from a string.
        /// </summary>
        /// <param name="scope">The scope in its urn string format - tenant:type:qualifier.</param>
        /// <returns>A parsed scope.</returns>
        public static GrantScope FromString(string scope)
        {
            var parts = scope.Split(':', 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                throw new ArgumentException("The scope is invalid. The expected format is [TenantId]:[GrantType]:[Qualifier].");
            }

            return new GrantScope(parts[0], parts[1], parts[2]);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{TenantId}:{GrantType}:{Qualifier}";
    }
}
