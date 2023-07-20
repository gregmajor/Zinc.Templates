using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using RedLine.A3.Authorization;
using RedLine.Application.Exceptions;
using RedLine.Data.A3.Authorization;
using RedLine.Domain.A3.Authorization.Repositories;

namespace RedLine.Application.Commands.Grants.RevokeGrant
{
    /// <summary>
    /// Handles the <see cref="RevokeGrantCommand" />.
    /// </summary>
    internal class RevokeGrantCommandHandler : IRequestHandler<RevokeGrantCommand, AuditableResult<bool>>
    {
        private readonly IGrantRepository repository;
        private readonly IDistributedCache cache;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="repository">The aggregate repository.</param>
        /// <param name="cache">A cache implementation.</param>
        public RevokeGrantCommandHandler(IGrantRepository repository, IDistributedCache cache)
        {
            this.repository = repository;
            this.cache = cache;
        }

        /// <inheritdoc />
        public async Task<AuditableResult<bool>> Handle(RevokeGrantCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId.Equals(request.AccessToken.UserId, StringComparison.OrdinalIgnoreCase))
            {
                throw NotAuthorizedException.Because(request.AccessToken, "Users are not allowed to administer their own grants.");
            }

            var key = new GrantKey(
                request.UserId,
                request.TenantId,
                request.GrantType,
                request.Qualifier);

            var grant = await repository.Read(key.ToString()).ConfigureAwait(false);

            if (grant == null)
            {
                return new AuditableResult<bool>(false, $"{request.AccessToken.FullName} failed to revoke grant {key.Scope} on {DateTime.Now.ToString("M/d/yyyy")} because the grant was not found and may have already been revoked.");
            }

            grant.Revoke(request.AccessToken.FullName);

            await repository.Delete(grant).ConfigureAwait(false);

            await cache.RemoveAsync(AuthorizationCacheKey.ForGrants(request.UserId), cancellationToken).ConfigureAwait(false);

            return new AuditableResult<bool>(true, $"Revoked from {grant.FullName} on {DateTime.Now.ToString("M/d/yyyy")} by {request.AccessToken.FullName}.");
        }
    }
}
