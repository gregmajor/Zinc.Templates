using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DOPA;
using Microsoft.Extensions.Caching.Distributed;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.Application.A3.Authorization.PolicyData;
using RedLine.Data.Serialization;
using RedLine.Domain;

namespace RedLine.Application.A3.Authorization
{
    /// <summary>
    /// Provides an implementation of <see cref="IAuthorizationPolicyProvider"/> interface.
    /// </summary>
    internal class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly IOpaPolicy<AuthorizationPolicy> opaPolicy;
        private readonly Activities activities;
        private readonly ActivityGroups activityGroups;
        private readonly Grants grants;
        private readonly IAuthenticationToken currentUser;
        private readonly IDistributedCache cache;
        private readonly ITenantId tenantId;

        public AuthorizationPolicyProvider(
            IOpaPolicy<AuthorizationPolicy> opaPolicy,
            Activities activities,
            ActivityGroups activityGroups,
            Grants grants,
            IAuthenticationToken currentUser,
            IDistributedCache cache,
            ITenantId tenantId)
        {
            this.opaPolicy = opaPolicy;
            this.activities = activities;
            this.activityGroups = activityGroups;
            this.grants = grants;

            this.currentUser = currentUser;
            this.cache = cache;
            this.tenantId = tenantId;
        }

        /// <inheritdoc />
        public async Task<IAuthorizationPolicy> GetPolicy()
        {
            await SetPolicyData(currentUser.UserId).ConfigureAwait(false);
            return new AuthorizationPolicy(opaPolicy, tenantId, currentUser.UserId);
        }

        private async Task SetPolicyData(string userId)
        {
            var authData = new
            {
                grants = await GetGrants(userId).ConfigureAwait(false),
                activityGroups = await GetActivityGroups().ConfigureAwait(false),
                activities = activities.Value,
            };

            opaPolicy.SetData(authData);
        }

        private async Task<IDictionary<string, object>> GetGrants(string userId)
        {
            var cachedGrants = await ReadFromCache(AuthorizationCacheKey.ForGrants(userId)).ConfigureAwait(false);

            if (cachedGrants == null)
            {
                cachedGrants = await grants.Value(userId).ConfigureAwait(false);
                await WriteToCache(AuthorizationCacheKey.ForGrants(userId), cachedGrants, TimeSpan.FromMinutes(5)).ConfigureAwait(false);
            }

            return cachedGrants;
        }

        private async Task<IDictionary<string, object>> GetActivityGroups()
        {
            var groups = await ReadFromCache(AuthorizationCacheKey.ForActivityGroups()).ConfigureAwait(false);

            if (groups == null)
            {
                groups = await activityGroups.Value().ConfigureAwait(false);
                await WriteToCache(AuthorizationCacheKey.ForActivityGroups(), groups, TimeSpan.FromHours(1)).ConfigureAwait(false);
            }

            return groups;
        }

        private async Task<IDictionary<string, object>> ReadFromCache(string key)
        {
            var item = await cache.GetStringAsync(key).ConfigureAwait(false);
            return item == null ? null : JsonSerializer.Deserialize<IDictionary<string, object>>(item, RedLineJsonSerializerOptions.Opa);
        }

        private Task WriteToCache(string key, IDictionary<string, object> item, TimeSpan slidingExpiration)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration,
            };
            return cache.SetStringAsync(key, JsonSerializer.Serialize(item, RedLineJsonSerializerOptions.Opa), options);
        }
    }
}
