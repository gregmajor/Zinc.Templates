using RedLine.A3;

namespace RedLine.Application
{
    /// <summary>
    /// The interface that defines a contract for commands and queries that must be authorized (i.e. those for which the current user must have an Activity grant).
    /// </summary>
    public interface IAmAuthorizable
    {
        /// <summary>
        /// The <see cref="IAccessToken"/> for the current user.
        /// </summary>
        IAccessToken AccessToken { get; set; }
    }
}
