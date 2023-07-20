using RedLine.A3.Authentication;

namespace RedLine.A3
{
    /// <summary>
    /// Extension methods for the <see cref="IAccessToken"/> interface.
    /// </summary>
    public static class TokenExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the current user is anonymous.
        /// </summary>
        /// <param name="user">The current user's <see cref="IAuthenticationToken"/>.</param>
        /// <returns>True if the current user is anonymous; otherwise, false.</returns>
        public static bool IsAnonymous(this IAuthenticationToken user)
        {
            return !user.IsAuthenticated();
        }

        /// <summary>
        /// Gets a value indicating whether the current user is authenticated.
        /// </summary>
        /// <param name="user">The current user's <see cref="IAuthenticationToken"/>.</param>
        /// <returns>True if the current user is authenticated; otherwise, false.</returns>
        public static bool IsAuthenticated(this IAuthenticationToken user)
        {
            return user.AuthenticationState == AuthenticationState.Authenticated;
        }
    }
}
