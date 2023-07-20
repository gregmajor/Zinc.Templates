using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Data.Exceptions;
using RedLine.Domain;

namespace RedLine.Application.A3.Authorization
{
    /// <inheritdoc />
    public class ActivityGroupService : IActivityGroupService
    {
        private readonly HttpClient httpClient;
        private readonly ICorrelationId correlationId;
        private readonly IDbConnection connection;
        private readonly IAuthenticationToken token;
        private readonly IDistributedCache cache;
        private readonly ILogger<ActivityGroupService> logger;

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="httpClient">Http client configured to talk to the activity group service.</param>
        /// <param name="correlationId">The <see cref="ICorrelationId"/> for the current request.</param>
        /// <param name="connection">The <see cref="IDbConnection"/> to the database.</param>
        /// <param name="token">The <see cref="IAuthenticationToken"/> for connecting to authz.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="cache">A cache implementation.</param>
        public ActivityGroupService(
            HttpClient httpClient,
            ICorrelationId correlationId,
            IDbConnection connection,
            IAuthenticationToken token,
            ILogger<ActivityGroupService> logger,
            IDistributedCache cache)
        {
            this.httpClient = httpClient;
            this.correlationId = correlationId;
            this.connection = connection;
            this.token = token;
            this.logger = logger;
            this.cache = cache;
        }

        /// <inheritdoc/>
        public async Task SyncActivityGroups()
        {
            var endpoint = $"api/v1/*/applications/{ApplicationContext.ApplicationName}/activity-groups"; // purposefully sending '*' as the tenant
            using var response = await httpClient.SendAsync(CreateMessage(endpoint)).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var reason = JsonConvert.DeserializeObject<string>(body);
                this.logger.LogError("Failed to retrieve activity groups because {reason}", reason);
                throw new OperationFailedException((int)response.StatusCode, "ActivityGroup", nameof(SyncActivityGroups), reason, null);
            }

            var activityGroups = JsonConvert.DeserializeObject<IEnumerable<ActivityGroup>>(body);
            await connection.ExecuteAsync("delete from authz.activity_group").ConfigureAwait(false);

            var activitiesInGroups = activityGroups.SelectMany(a => a.Activities.Select(act => new { ActivityGroupName = a.Name, ActivityName = act.ActivityName, TenantId = a.TenantId })).ToArray();

            if (activitiesInGroups.Any())
            {
                await connection.ExecuteAsync("insert into authz.activity_group(tenant_id, name, activity_name) values (@TenantId, @ActivityGroupName, @ActivityName);", activitiesInGroups).ConfigureAwait(false);
            }

            await cache.RemoveAsync(AuthorizationCacheKey.ForActivityGroups()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SyncActivityGroupGrants(string userId)
        {
            var endpoint = $"api/v1/*/{userId}/grants/activity-groups"; // purposefully sending '*' as the tenant
            using var response = await httpClient.SendAsync(CreateMessage(endpoint)).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var reason = JsonConvert.DeserializeObject<string>(body);
                this.logger.LogError("Failed to retrieve activity group grants because {reason}", reason);
                throw new OperationFailedException((int)response.StatusCode, "ActivityGroupGrant", nameof(SyncActivityGroupGrants), reason, null);
            }

            var results = JsonConvert.DeserializeObject<IEnumerable<ActivityGroupGrant>>(body);
            var grants = results.Select(a => new
            {
                UserId = userId,
                TenantId = a.TenantId,
                GrantType = GrantType.ActivityGroup,
                Qualifier = a.ActivityGroupName,
                ExpiresOn = a.ExpiresOn,
                GrantedBy = token.FullName,
            }).ToArray();

            await connection.ExecuteAsync($"delete from authz.grant where user_id=@userId and grant_type='{GrantType.ActivityGroup}'", new { userId }).ConfigureAwait(false);

            if (grants.Any())
            {
                await connection.ExecuteAsync($"insert into authz.grant(user_id, full_name, tenant_id, grant_type, qualifier, expires_on, granted_by, granted_on) values (@UserId, @UserId, @TenantId, @GrantType, @Qualifier, @ExpiresOn, @GrantedBy, now() at time zone 'utc');", grants).ConfigureAwait(false);
            }

            await cache.RemoveAsync(AuthorizationCacheKey.ForGrants(userId)).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task SyncAllActivityGroupGrants()
        {
            var endpoint = $"api/v1/*/grants/activity-groups"; // purposefully sending '*' as the tenant
            using var response = await httpClient.SendAsync(CreateMessage(endpoint)).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var reason = JsonConvert.DeserializeObject<string>(body);
                this.logger.LogError("Failed to retrieve activity group grants because {reason}", reason);
                throw new OperationFailedException((int)response.StatusCode, "ActivityGroupGrant", nameof(SyncAllActivityGroupGrants), reason, null);
            }

            var results = JsonConvert.DeserializeObject<IEnumerable<ActivityGroupGrant>>(body);

            var grants = results.Select(a => new
            {
                UserId = a.UserId,
                TenantId = a.TenantId,
                GrantType = GrantType.ActivityGroup,
                Qualifier = a.ActivityGroupName,
                ExpiresOn = a.ExpiresOn,
                GrantedBy = token.FullName,
            }).ToArray();

            var userIds = await connection.QueryAsync<string>($"select distinct user_id from authz.grant where grant_type='{GrantType.ActivityGroup}'").ConfigureAwait(false);
            await connection.ExecuteAsync($"delete from authz.grant where grant_type='{GrantType.ActivityGroup}'").ConfigureAwait(false);

            if (grants.Any())
            {
                await connection.ExecuteAsync($"insert into authz.grant(user_id, full_name, tenant_id, grant_type, qualifier, expires_on, granted_by, granted_on) values (@UserId, @UserId, @TenantId, @GrantType, @Qualifier, @ExpiresOn, @GrantedBy, now() at time zone 'utc');", grants).ConfigureAwait(false);
            }

            await Task.WhenAll(userIds.Select(u => cache.RemoveAsync(AuthorizationCacheKey.ForGrants(u)))).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task<bool> Exists(string activityGroupName) =>
            connection.ExecuteScalarAsync<bool>("select exists (select 1 from authz.activity_group where name=@activityGroupName);", new { activityGroupName });

        private HttpRequestMessage CreateMessage(string endpoint)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, endpoint);

            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Jwt ?? string.Empty);
            message.Headers.Add(RedLineHeaderNames.CorrelationId, correlationId.ToString());

            return message;
        }

        [SuppressMessage("Design", "S3459: Remove unassigned auto-property, or set its value.", Justification = "For deserialization.")]
        private sealed record ActivityGroup
        {
            public string TenantId { get; init; }

            public string Name { get; init; }

            public string Description { get; init; }

            public List<Activity> Activities { get; init; }
        }

        [SuppressMessage("Design", "S3459: Remove unassigned auto-property, or set its value.", Justification = "For deserialization.")]
        private sealed record Activity
        {
            public string ApplicationName { get; init; }

            public string ActivityName { get; init; }
        }

        [SuppressMessage("Design", "S3459: Remove unassigned auto-property, or set its value.", Justification = "For deserialization.")]
        private sealed record ActivityGroupGrant
        {
            public string TenantId { get; init; }

            public string ActivityGroupName { get; init; }

            public DateTimeOffset? ExpiresOn { get; init; }

            public string UserId { get; init; }
        }
    }
}
