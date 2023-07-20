using System;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Application.A3.Authorization;
using RedLine.Data.Exceptions;
using RedLine.Domain;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.FunctionalTests.Mothers;
using static Zinc.Templates.FunctionalTests.Mothers.MessageHandlerMother;

namespace Zinc.Templates.FunctionalTests.Application.A3.Authorization.ActivityGroupServiceTests
{
    public class SyncAllActivityGroupGrantsShould : FunctionalTestBase
    {
        private readonly IDistributedCache cache;
        private readonly IDbConnection connection;
        private readonly ICorrelationId correlationId;
        private readonly IAuthenticationToken token;
        private readonly ILogger<ActivityGroupService> logger;

        public SyncAllActivityGroupGrantsShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            this.cache = GetRequiredService<IDistributedCache>();
            this.connection = GetRequiredService<IDbConnection>();
            this.correlationId = GetRequiredService<ICorrelationId>();
            this.token = GetRequiredService<IAuthenticationToken>();
            this.logger = GetRequiredService<ILogger<ActivityGroupService>>();
        }

        [Fact]
        public async Task PersistGrantsForAllUsers()
        {
            // Arrange
            var userId1 = "userId1";
            var userId2 = "userId2";
            var returnedGrants = new[]
            {
                new { TenantId = TenantId, ActivityGroupName = "ActGroup1", UserId = userId1 },
                new { TenantId = TenantId, ActivityGroupName = "ActGroup1", UserId = userId2 },
            };
            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", returnedGrants);
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncAllActivityGroupGrants().ConfigureAwait(false);

            // Assert
            var grants = (await connection.QueryAsync<(string UserId, string TenantId, string GrantType, string Qualifier)>(
                $"select user_id, tenant_id, grant_type, qualifier from authz.grant where grant_type='{GrantType.ActivityGroup}' and user_id=@userId1 or user_id=@userId2", new { userId1, userId2 }).ConfigureAwait(false))
                .Select(x => new
                {
                    TenantId = x.TenantId,
                    ActivityGroupName = x.Qualifier,
                    UserId = x.UserId,
                });

            grants.Should().HaveCount(returnedGrants.Count());
            grants.Should().BeEquivalentTo(returnedGrants);
        }

        [Fact]
        public async Task ClearExistingCacheForAllUsers()
        {
            // Arrange
            var userId1 = "userId1";
            var userId2 = "userId2";
            await cache.SetStringAsync(AuthorizationCacheKey.ForGrants(userId1), "{}").ConfigureAwait(false);
            await cache.SetStringAsync(AuthorizationCacheKey.ForGrants(userId2), "{}").ConfigureAwait(false);
            var existingGrants = new[]
            {
                new
                {
                    UserId = userId1,
                    TenantId = TenantId,
                    GrantType = GrantType.ActivityGroup,
                    Qualifier = "ActGroup1",
                    GrantedBy = token.FullName,
                },
                new
                {
                    UserId = userId2,
                    TenantId = TenantId,
                    GrantType = GrantType.ActivityGroup,
                    Qualifier = "ActGroup1",
                    GrantedBy = token.FullName,
                },
            };

            await connection.ExecuteAsync($"insert into authz.grant(user_id, full_name, tenant_id, grant_type, qualifier, expires_on, granted_by, granted_on) values (@UserId, @UserId, @TenantId, @GrantType, @Qualifier, NULL, @GrantedBy, now() at time zone 'utc');", existingGrants).ConfigureAwait(false);

            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", new object[] { });
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncAllActivityGroupGrants().ConfigureAwait(false);

            // Assert
            var cached = await cache.GetStringAsync(AuthorizationCacheKey.ForGrants(userId1)).ConfigureAwait(false);
            cached.Should().BeNull();
            cached = await cache.GetStringAsync(AuthorizationCacheKey.ForGrants(userId2)).ConfigureAwait(false);
            cached.Should().BeNull();
        }

        [Fact]
        public async Task ThrowOperationFailed()
        {
            // Arrange
            var handler = MessageHandlerMother.NewHandler();
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            Func<Task> act = () => service.SyncAllActivityGroupGrants();

            // Assert
            await act.Should().ThrowAsync<OperationFailedException>().ConfigureAwait(false);
        }

        private HttpClient GetHttpClient(MessageHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(ApplicationContext.AuthorizationServiceEndpoint),
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
