namespace RedLine.A3.Authentication
{
    /// <summary>
    /// An enum that defines various authentication states.
    /// </summary>
    public enum AuthenticationState
    {
        /// <summary>
        /// The current user is anonymous.
        /// </summary>
        Anonymous,

        /// <summary>
        /// The current user is authenticated.
        /// </summary>
        Authenticated,

        /// <summary>
        /// The current user has been identified, such as through a stored cookie, but has not been
        /// challenged to provide credentials. In other words, we think we know who it is, but their
        /// identity has not been proven by challenging them for credentials.
        /// </summary>
        Identified,
    }
}
