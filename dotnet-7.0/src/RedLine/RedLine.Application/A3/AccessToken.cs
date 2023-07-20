using RedLine.A3.Authentication;
using RedLine.A3.Authorization;

namespace RedLine.A3
{
    /// <summary>
    /// Provides an implementation of the <see cref="IAccessToken"/> interface.
    /// </summary>
    public class AccessToken : IAccessToken
    {
        private readonly IAuthenticationToken authenticationToken;
        private readonly IAuthorizationPolicy authorizationPolicy;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="authenticationToken">The <see cref="IAuthenticationToken"/> for the current user.</param>
        /// <param name="authorizationPolicy">The <see cref="IAuthorizationPolicy"/> for the current user.</param>
        public AccessToken(IAuthenticationToken authenticationToken, IAuthorizationPolicy authorizationPolicy)
        {
            if (authenticationToken.UserId != authorizationPolicy.UserId)
            {
                throw new Domain.Exceptions.DomainException(400, "The IAuthenticationToken and IAuthorizationToken are for different users.");
            }

            this.authenticationToken = authenticationToken;
            this.authorizationPolicy = authorizationPolicy;
        }

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState => authenticationToken.AuthenticationState;

        /// <inheritdoc/>
        string IAuthenticationToken.Jwt { get => authenticationToken.Jwt; set => authenticationToken.Jwt = value; }

        /// <inheritdoc/>
        public string Login => authenticationToken.Login;

        /// <inheritdoc/>
        public string TenantId => authorizationPolicy.TenantId;

        /// <inheritdoc/>
        public string UserId => authenticationToken.UserId;

        /// <inheritdoc/>
        public string FullName => authenticationToken.FullName;

        /// <inheritdoc/>
        public string FirstName => authenticationToken.FirstName;

        /// <inheritdoc/>
        public string LastName => authenticationToken.LastName;

        /// <inheritdoc/>
        public bool IsAuthorized(string activityName, string resourceId = null) => authorizationPolicy.IsAuthorized(activityName, resourceId);

        /// <inheritdoc/>
        public bool HasGrant(string activityName) => authorizationPolicy.HasGrant(activityName);

        /// <inheritdoc/>
        public bool HasGrant<TActivity>() => authorizationPolicy.HasGrant<TActivity>();

        /// <inheritdoc/>
        public bool HasGrant(string resourceType, string resourceId) => authorizationPolicy.HasGrant(resourceType, resourceId);

        /// <inheritdoc/>
        public bool HasGrant<TResourceType>(string resourceId) => authorizationPolicy.HasGrant<TResourceType>(resourceId);
    }
}
