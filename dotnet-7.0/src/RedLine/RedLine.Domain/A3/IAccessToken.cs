using RedLine.A3.Authentication;
using RedLine.A3.Authorization;

namespace RedLine.A3
{
    /// <summary>
    /// The interface that defines a contract for a RedLine access token, which is semantically
    /// equivalent to a <see cref="System.Security.Claims.ClaimsPrincipal"/>, in that it contains
    /// both the identity and the authorization data for the current user.
    /// </summary>
    public interface IAccessToken : IAuthenticationToken, IAuthorizationPolicy
    {
        /// <summary>
        /// Gets the immutable id of the user defined by the identity provider, in the form of 'subject@issuer'.
        /// </summary>
        new string UserId { get; }
    }
}
