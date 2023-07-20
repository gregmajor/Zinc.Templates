using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.A3;
using RedLine.Application.Exceptions;

namespace RedLine.Application.Behaviors
{
    /// <summary>
    /// A behavior used to authorize requests.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    internal class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IAccessToken accessToken;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="accessToken">The <see cref="IAccessToken"/> for the current request.</param>
        public AuthorizationBehavior(IAccessToken accessToken)
        {
            this.accessToken = accessToken;
        }

        /// <summary>
        /// Applies the behavior and executes the next one in the pipeline.
        /// </summary>
        /// <param name="request">The executing request.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the request.</param>
        /// <param name="next">The next behavior in the pipeline.</param>
        /// <returns>The response as a <typeparamref name="TResponse"/>.</returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is not IAmAuthorizable authorizable)
            {
                return await next().ConfigureAwait(false);
            }

            if (accessToken.IsAnonymous())
            {
                throw NotAuthorizedException.BecauseNotAuthenticated();
            }

            var activityName = request.GetType().Name;
            var resourceId = (request as IAmAuthorizableForResource)?.ResourceId;

            if (!accessToken.IsAuthorized(activityName, resourceId))
            {
                throw NotAuthorizedException.BecauseForbidden(accessToken, activityName, resourceId);
            }

            authorizable.AccessToken = accessToken;
            return await next().ConfigureAwait(false);
        }
    }
}
