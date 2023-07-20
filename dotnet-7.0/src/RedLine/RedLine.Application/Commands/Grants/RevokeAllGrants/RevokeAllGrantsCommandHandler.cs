using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Application.Exceptions;
using RedLine.Domain.A3.Authorization.Repositories;

namespace RedLine.Application.Commands.Grants.RevokeAllGrants
{
    /// <summary>
    /// A handler for the <see cref="RevokeAllGrantsCommand"/>.
    /// </summary>
    internal class RevokeAllGrantsCommandHandler : IRequestHandler<RevokeAllGrantsCommand, AuditableResult<bool>>
    {
        private readonly IGrantRepository repository;
        private readonly IDistributedCache cache;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="repository">A repository for grants.</param>
        /// <param name="cache">A cache implementation.</param>
        public RevokeAllGrantsCommandHandler(IGrantRepository repository, IDistributedCache cache)
        {
            this.repository = repository;
            this.cache = cache;
        }

        /// <summary>
        /// Handles the <see cref="RevokeAllGrantsCommand"/>.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>True if the grant was revoked; otherwise, false.</returns>
        public async Task<AuditableResult<bool>> Handle(RevokeAllGrantsCommand request, CancellationToken cancellationToken)
        {
            if (request.AccessToken.UserId.Equals(request.UserId, StringComparison.OrdinalIgnoreCase))
            {
                throw NotAuthorizedException.Because(request.AccessToken, "Users are not allowed to administer their own grants.");
            }

            var actor = request.AccessToken.FullName;

            /* NOTE: We only care about application grants here and not activity group grants
             * because the activity group service (AuthZ) will handle those.
             * */
            var applicationGrants = (await repository.ReadAll(request.UserId).ConfigureAwait(false))
                .Where(g => g.Scope.GrantType != GrantType.ActivityGroup);

            foreach (var grant in applicationGrants)
            {
                // If the grant is already expired, no need to revoke it.
                if (!grant.IsExpired())
                {
                    grant.Revoke(actor);
                }

                await repository.Delete(grant).ConfigureAwait(false);
            }

            await cache.RemoveAsync(AuthorizationCacheKey.ForGrants(request.UserId), cancellationToken).ConfigureAwait(false);

            if (applicationGrants.Any())
            {
                return new AuditableResult<bool>(true, $"Revoked from {request.FullName} on {DateTime.Now:M/d/yyyy} by {actor}.");
            }

            return new AuditableResult<bool>(false, $"No grants were found to revoke from {request.FullName}.");
        }
    }
}
