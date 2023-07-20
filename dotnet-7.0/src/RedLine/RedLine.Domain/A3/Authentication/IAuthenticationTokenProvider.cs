using System.Threading.Tasks;

namespace RedLine.A3.Authentication
{
    /// <summary>
    /// The interface that defines a contract for obtaining an <see cref="IAuthenticationToken"/>.
    /// </summary>
    public interface IAuthenticationTokenProvider
    {
        /* PERHAPS AT SOME POINT IN THE FUTURE (for now clients login using the OIDC flow):
        /// <summary>
        /// Gets <see cref="IAuthenticationToken"/> for the current client.
        /// </summary>
        /// <returns>The <see cref="IAuthenticationToken"/> for the current client.</returns>
        Task<IAuthenticationToken> GetClientAuthenticationToken();.
        */

        /// <summary>
        /// Authenticates the current service using the OAuth "client credentials" flow.
        /// This type of authentication allows for service-to-service calls, or when the
        /// service itself is performing operations, such as when executing a job.
        /// </summary>
        /// <remarks>
        /// The current implementation authenticates based on a service certificate.
        /// </remarks>
        /// <returns>The <see cref="IAuthenticationToken"/> for the current service.</returns>
        Task<IAuthenticationToken> GetServiceAuthenticationToken();
    }
}
