using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Application.Exceptions;
using RedLine.Data.A3.Authorization;
using RedLine.Domain.A3.Authorization.Repositories;

namespace RedLine.Application.Commands.Grants.AddGrant
{
    /// <summary>
    /// Handles the <see cref="AddGrantCommand" />.
    /// </summary>
    internal class AddGrantCommandHandler : IRequestHandler<AddGrantCommand, AuditableResult<bool>>
    {
        private readonly IGrantRepository repository;
        private readonly IDistributedCache cache;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="repository">The aggregate repository.</param>
        /// <param name="cache">A cache implementation.</param>
        public AddGrantCommandHandler(IGrantRepository repository, IDistributedCache cache)
        {
            this.repository = repository;
            this.cache = cache;
        }

        /// <inheritdoc />
        public async Task<AuditableResult<bool>> Handle(AddGrantCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId.Equals(request.AccessToken.UserId, System.StringComparison.OrdinalIgnoreCase))
            {
                throw NotAuthorizedException.Because(request.AccessToken, "Users are not allowed to administer their own grants.");
            }

            var key = new GrantKey(request.UserId, request.TenantId, request.GrantType, request.Qualifier);

            var grant = await repository.Read(key.ToString()).ConfigureAwait(false);

            if (grant != null)
            {
                if (grant.IsExpired())
                {
                    await repository.Delete(grant).ConfigureAwait(false);
                }
                else
                {
                    return new AuditableResult<bool>(false, $"{request.AccessToken.FullName} failed to grant {request.FullName} on {DateTime.Now.ToString("M/d/yyyy")} because the grant was already in effect.");
                }
            }

            grant = new Grant(
                request.UserId,
                request.FullName,
                request.TenantId,
                request.GrantType,
                request.Qualifier,
                request.ExpiresOn,
                request.AccessToken.FullName);

            await repository.Save(grant).ConfigureAwait(false);

            await cache.RemoveAsync(AuthorizationCacheKey.ForGrants(request.UserId), cancellationToken).ConfigureAwait(false);

            return new AuditableResult<bool>(true, $"Granted to {request.FullName} on {DateTime.Now.ToString("M/d/yyyy")} by {request.AccessToken.FullName}.");
        }
    }
}
