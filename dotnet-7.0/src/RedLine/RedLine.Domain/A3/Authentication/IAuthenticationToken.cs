namespace RedLine.A3.Authentication
{
    /// <summary>
    /// The interface that defines a contract for an authenticated user.
    /// </summary>
    public interface IAuthenticationToken
    {
        /// <summary>
        /// Gets the <see cref="Authentication.AuthenticationState"/> of the current user.
        /// </summary>
        AuthenticationState AuthenticationState { get; }

        /// <summary>
        /// Gets the raw JWT bearer token for the current user.
        /// </summary>
        string Jwt { get; set;  }

        /// <summary>
        /// Gets the first name of the user.
        /// </summary>
        string FirstName { get; }

        /// <summary>
        /// Gets the full name, or display name of the user.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the last name of the user.
        /// </summary>
        string LastName { get; }

        /// <summary>
        /// Gets the user login, typically an email address.
        /// </summary>
        string Login { get; }

        /// <summary>
        /// Gets the immutable id of the user defined by the identity provider, in the form of 'subject@issuer'.
        /// </summary>
        string UserId { get; }
    }
}
