using System;
using System.Diagnostics;
using DOPA;
using RedLine.A3.Authorization;
using RedLine.Domain;

namespace RedLine.Application.A3.Authorization
{
    /// <summary>
    /// The authorization policy for the current context.
    /// </summary>
    internal class AuthorizationPolicy : IAuthorizationPolicy, IDisposable
    {
        private readonly IOpaPolicy<AuthorizationPolicy> policy;

        /// <summary>
        /// Initializes the authorization policy. This is not automatically resolved by DI.
        /// </summary>
        /// <param name="policy">The provider to get the policy.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="userId">The user this policy applies to.</param>
        public AuthorizationPolicy(IOpaPolicy<AuthorizationPolicy> policy, ITenantId tenantId, string userId)
        {
            this.policy = policy;
            TenantId = tenantId.Value;
            UserId = userId;
        }

        /// <inheritdoc />
        public string TenantId { get; }

        /// <inheritdoc />
        public string UserId { get; }

        /// <inheritdoc />
        public bool IsAuthorized(string activityName, string resourceId = null)
        {
            var result = Evaluate(activityName, resourceId, null);

            return result?.IsAuthorized ?? false;
        }

        /// <inheritdoc />
        public bool HasGrant(string activityName)
        {
            var result = Evaluate(activityName, null, null);

            return result?.HasActivityGrant ?? false;
        }

        /// <inheritdoc />
        public bool HasGrant<TActivity>()
        {
            var activity = TypeNameHelper.GetTypeDisplayName(typeof(TActivity), false, true);
            return HasGrant(activity);
        }

        /// <inheritdoc />
        public bool HasGrant(string resourceType, string resourceId)
        {
            var result = Evaluate(null, resourceId, resourceType);

            return result?.HasResourceGrant ?? false;
        }

        /// <inheritdoc />
        public bool HasGrant<TResourceType>(string resourceId)
        {
            var resourceType = TypeNameHelper.GetTypeDisplayName(typeof(TResourceType), false, true);
            return HasGrant(resourceType, resourceId);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the thing.
        /// </summary>
        /// <param name="disposing">Are we disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                policy?.Dispose();
            }
        }

        private PolicyResult Evaluate(string activity, string resource, string resourceType)
        {
            var input = new
            {
                TenantId,
                activity,
                resource,
                resourceType,
                now = DateTime.UtcNow.Ticks,
            };

            return policy.Evaluate<PolicyResult>(input);
        }

        internal record OpaResult
        {
            public PolicyResult Result { get; init; }
        }

        internal record PolicyResult
        {
            public bool IsAuthorized { get; init; }

            public bool HasActivityGrant { get; init; }

            public bool HasResourceGrant { get; init; }
        }
    }
}
