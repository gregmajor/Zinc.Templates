using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Application.Commands.Grants.AddGrant;
using RedLine.Data.Serialization;
using RedLine.Domain.A3.Authorization.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Application.A3.Authorization.AuthorizationPolicyProviderTests
{
    public class GetPolicyShould : FunctionalTestBase
    {
        private readonly IDistributedCache cache;
        private readonly IAuthenticationToken user;
        private readonly IAuthorizationPolicyProvider provider;
        private readonly IGrantRepository grantRepository;
        private readonly IDbConnection connection;

        public GetPolicyShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            cache = GetRequiredService<IDistributedCache>();
            user = GetRequiredService<IAuthenticationToken>();
            provider = GetRequiredService<IAuthorizationPolicyProvider>();
            grantRepository = GetRequiredService<IGrantRepository>();
            connection = GetRequiredService<IDbConnection>();
        }

        [Fact]
        public async Task ReturnPolicyWithoutCachedGrants()
        {
            // Arrange
            var activity = "RandomActivity";
            await AddGrant(GrantType.Activity, activity).ConfigureAwait(false);
            await RemoveCache().ConfigureAwait(false);

            // Act
            var policy = await provider.GetPolicy().ConfigureAwait(false);

            // Assert
            policy.IsAuthorized(activity).Should().BeTrue();
        }

        [Fact]
        public async Task ReturnPolicyWithoutCachedActivityGroups()
        {
            // Arrange
            var group = "RandomGroup";
            var activity = nameof(AddGrantCommand);
            await AddGrant(GrantType.ActivityGroup, group).ConfigureAwait(false);
            await connection.ExecuteAsync("insert into authz.activity_group (tenant_id, name, activity_name) values (@TenantId, @group, @activity)", new { TenantId, group, activity }).ConfigureAwait(false);
            await RemoveCache().ConfigureAwait(false);

            // Act
            var policy = await provider.GetPolicy().ConfigureAwait(false);

            // Assert
            policy.IsAuthorized(activity).Should().BeTrue();
        }

        [Fact]
        public async Task ReturnPolicyWithCachedActivityGroups()
        {
            // Arrange
            var activity = nameof(AddGrantCommand);
            var group = "RandomGroup";
            await AddCache(AuthorizationCacheKey.ForActivityGroups(), new() { { group, new { Activities = new[] { new { ActivityName = activity, TenantId = TenantId } } } } }).ConfigureAwait(false);
            await AddCache(AuthorizationCacheKey.ForGrants(user.UserId), new() { { $"{TenantId}:ActivityGroup:{group}", new { ExpiresOn = (DateTimeOffset?)null } } }).ConfigureAwait(false);

            // Act
            var policy = await provider.GetPolicy().ConfigureAwait(false);

            // Assert
            policy.IsAuthorized(activity).Should().BeTrue();
        }

        private Task AddGrant(string type, string qualifier)
        {
            var grant = new Grant(user.UserId, user.FullName, TenantId, type, qualifier, null, "me");
            return grantRepository.Save(grant);
        }

        private async Task RemoveCache()
        {
            await this.cache.RemoveAsync(AuthorizationCacheKey.ForGrants(user.UserId)).ConfigureAwait(false);
            await this.cache.RemoveAsync(AuthorizationCacheKey.ForActivityGroups()).ConfigureAwait(false);
        }

        private Task AddCache(string key, Dictionary<string, object> item)
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(5),
            };
            return cache.SetStringAsync(key, JsonSerializer.Serialize(item, RedLineJsonSerializerOptions.Opa), options);
        }
    }
}
