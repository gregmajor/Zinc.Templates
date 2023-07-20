using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using RedLine.A3.Authorization.Domain;
using RedLine.Data.A3.Authorization;
using RedLine.Data.Exceptions;
using RedLine.Data.Outbox;
using RedLine.Data.Repositories;
using RedLine.Domain;
using RedLine.Domain.A3.Authorization.Repositories;
using RedLine.Domain.Exceptions;

namespace RedLine.A3.Authorization
{
    /// <summary>
    /// A repository for the <see cref="Grant"/> aggregate.
    /// </summary>
    public class GrantRepository : DbRepositoryBase<Grant>, IGrantRepository
    {
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="outbox">The outbox where domain events are written.</param>
        public GrantRepository(IActivityContext context, IOutbox outbox)
            : base(context, outbox)
        {
            connection = context.Connection();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Grant>> Matching(GrantScope scope, string userId = null)
        {
            var grants = await connection.QueryAsync<GrantData>(
                Sql.Matching,
                new
                {
                    UserId = userId,
                    TenantId = scope.TenantId,
                    GrantType = scope.GrantType,
                    Qualifier = scope.Qualifier,
                }).ConfigureAwait(false);

            return grants
                .Select(g => new Grant(
                    g.UserId,
                    g.FullName,
                    g.TenantId,
                    g.GrantType,
                    g.Qualifier,
                    g.ExpiresOn,
                    g.GrantedBy,
                    g.GrantedOn.Value))
                .ToList();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Grant>> ReadAll(string userId)
        {
            try
            {
                var grants = await connection.QueryAsync<GrantData>(
                    Sql.ReadAll,
                    new { UserId = userId }).ConfigureAwait(false);

                return grants
                    .Select(g => new Grant(
                        g.UserId,
                        g.FullName,
                        g.TenantId,
                        g.GrantType,
                        g.Qualifier,
                        g.ExpiresOn,
                        g.GrantedBy,
                        g.GrantedOn.Value))
                    .ToList();
            }
            catch (Exception e)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(Grant), false, false),
                    nameof(ReadAll),
                    e);
            }
        }

        /// <inheritdoc/>
        protected override Task<int> DeleteInternal(Grant aggregate, string etag)
        {
            if (!(aggregate.IsRevoked() || aggregate.IsExpired()))
            {
                throw new DomainException(400, "A grant must be revoked or expired before deleting it.");
            }

            return connection.ExecuteAsync(
                Sql.Delete,
                new
                {
                    UserId = aggregate.UserId,
                    TenantId = aggregate.Scope.TenantId,
                    GrantType = aggregate.Scope.GrantType,
                    Qualifier = aggregate.Scope.Qualifier,
                    RevokedBy = aggregate.RevokedBy,
                    RevokedOn = aggregate.RevokedOn.HasValue ? aggregate.RevokedOn.Value : (DateTimeOffset?)null,
                });
        }

        /// <inheritdoc/>
        protected override Task<bool> ExistsInternal(string key)
        {
            var id = new GrantKey(key);

            return connection.ExecuteScalarAsync<bool>(
                Sql.Exists,
                new
                {
                    UserId = id.UserId,
                    TenantId = id.Scope.TenantId,
                    GrantType = id.Scope.GrantType,
                    Qualifier = id.Scope.Qualifier,
                });
        }

        /// <inheritdoc/>
        protected override async Task<Grant> ReadInternal(string key)
        {
            var id = new GrantKey(key);

            var grant = await connection.QuerySingleOrDefaultAsync<GrantData>(
                Sql.Read,
                new
                {
                    UserId = id.UserId,
                    TenantId = id.Scope.TenantId,
                    GrantType = id.Scope.GrantType,
                    Qualifier = id.Scope.Qualifier,
                }).ConfigureAwait(false);

            return grant == null
                ? null
                : new Grant(
                    grant.UserId,
                    grant.FullName,
                    grant.TenantId,
                    grant.GrantType,
                    grant.Qualifier,
                    grant.ExpiresOn,
                    grant.GrantedBy,
                    grant.GrantedOn.Value);
        }

        /// <inheritdoc/>
        protected override async Task<int> SaveInternal(Grant aggregate, string etag, string newETag)
        {
            if (!aggregate.IsActive())
            {
                throw new DomainException(400, "Only active grants can be saved.");
            }

            try
            {
                return await connection.ExecuteAsync(
                    Sql.Save,
                    new
                    {
                        UserId = aggregate.UserId,
                        FullName = aggregate.FullName,
                        TenantId = aggregate.Scope.TenantId,
                        GrantType = aggregate.Scope.GrantType,
                        Qualifier = aggregate.Scope.Qualifier,
                        ExpiresOn = aggregate.ExpiresOn,
                        GrantedBy = aggregate.GrantedBy,
                        GrantedOn = aggregate.GrantedOn,
                    }).ConfigureAwait(false);
            }
            catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UniqueViolation && e.ConstraintName == "grant_key")
            {
                throw new ResourceAlreadyExistsException(nameof(Grant), nameof(Save), e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3459:Unassigned members should be removed", Justification = "Dapper sets it.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Dapper sets it.")]
        private sealed record GrantData
        {
            public string UserId { get; init; }

            public string FullName { get; init; }

            public string TenantId { get; init; }

            public string GrantType { get; init; }

            public string Qualifier { get; init; }

            public DateTimeOffset? ExpiresOn { get; init; }

            public string GrantedBy { get; init; }

            public DateTimeOffset? GrantedOn { get; init; }
        }

        private static class Sql
        {
            public static readonly string Delete = @"
INSERT INTO authz.grant_history (
    user_id,
    full_name,
    tenant_id,
    grant_type,
    qualifier,
    expires_on,
    granted_by,
    granted_on,
    revoked_by,
    revoked_on
)
SELECT
    user_id,
    full_name,
    tenant_id,
    grant_type,
    qualifier,
    expires_on,
    granted_by,
    granted_on,
    @RevokedBy AS revoked_by,
    @RevokedOn::timestamptz AS revoked_on
FROM authz.grant
WHERE user_id = @UserId AND tenant_id = @TenantId AND grant_type = @GrantType AND qualifier = @Qualifier;

DELETE FROM authz.grant WHERE user_id = @UserId AND tenant_id = @TenantId AND grant_type = @GrantType AND qualifier = @Qualifier;"
;

            public static readonly string Exists = @"
SELECT EXISTS (
    SELECT 1
    FROM authz.grant
    WHERE user_id = @UserId
    AND tenant_id = @TenantId
    AND grant_type = @GrantType
    AND qualifier = @Qualifier
    AND COALESCE(expires_on, 'infinity') > now() at time zone 'utc'
);";

            public static readonly string Matching = @"
SELECT *
FROM authz.grant
WHERE user_id LIKE COALESCE(@UserId, '%')
AND tenant_id LIKE @TenantId
AND grant_type LIKE @GrantType
AND qualifier LIKE @Qualifier;";

            public static readonly string Read = @"
SELECT *
FROM authz.grant
WHERE user_id = @UserId
AND tenant_id = @TenantId
AND grant_type = @GrantType
AND qualifier = @Qualifier;";

            public static readonly string ReadAll = @"
SELECT *
FROM authz.grant
WHERE user_id = @UserId;";

            public static readonly string Save = @"
INSERT INTO authz.""grant"" (
    user_id,
    full_name,
    tenant_id,
    grant_type,
    qualifier,
    expires_on,
    granted_by,
    granted_on
) VALUES (
    @UserId,
    @FullName,
    @TenantId,
    @GrantType,
    @Qualifier,
    @ExpiresOn::timestamptz,
    @GrantedBy,
    @GrantedOn::timestamptz
);
";
        }
    }
}
