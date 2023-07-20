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
    public class SyncActivityGroupGrantsShould : FunctionalTestBase
    {
        private readonly IDistributedCache cache;
        private readonly IDbConnection connection;
        private readonly ICorrelationId correlationId;
        private readonly IAuthenticationToken token;
        private readonly ILogger<ActivityGroupService> logger;

        public SyncActivityGroupGrantsShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            this.cache = GetRequiredService<IDistributedCache>();
            this.connection = GetRequiredService<IDbConnection>();
            this.correlationId = GetRequiredService<ICorrelationId>();
            this.token = GetRequiredService<IAuthenticationToken>();
            this.logger = GetRequiredService<ILogger<ActivityGroupService>>();
        }

        [Fact]
        public async Task PersistGrantsForUser()
        {
            // Arrange
            var userId = "userId";
            var returnedGrants = new[]
            {
                new
                {
                    TenantId = TenantId,
                    ActivityGroupName = "ActGroup1",
                },
            };
            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", returnedGrants);
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncActivityGroupGrants(userId).ConfigureAwait(false);

            // Assert
            var grants = (await connection.QueryAsync<(string TenantId, string Qualifier)>(
                $"select tenant_id, qualifier from authz.grant where grant_type='{GrantType.ActivityGroup}' and user_id=@userId", new { userId }).ConfigureAwait(false))
                .Select(x => new
                {
                    TenantId = x.TenantId,
                    ActivityGroupName = x.Qualifier,
                })
                .ToList();

            grants.Should().HaveCount(returnedGrants.Count());
            grants.Should().BeEquivalentTo(returnedGrants);
        }

        [Fact]
        public async Task ReplaceExistingGrantsForUser()
        {
            // Arrange
            var userId = "userId";
            var sql = $"insert into authz.grant(user_id, full_name, tenant_id, grant_type, qualifier, granted_by, granted_on) values (@UserId, 'full name', '{TenantId}', 'ActivityGroup', 'SomeOtherActivityGroup::Name', 'system', now() at time zone 'utc');";
            await connection.ExecuteAsync(sql, new { userId }).ConfigureAwait(false);
            var returnedGrants = new[]
            {
                new { TenantId = TenantId, ActivityGroupName = "ActGroup1" },
            };
            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", returnedGrants);
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncActivityGroupGrants(userId).ConfigureAwait(false);

            // Assert
            var grants = (await connection.QueryAsync<(string TenantId, string Qualifier)>("select tenant_id, qualifier from authz.grant where grant_type='ActivityGroup' and user_id=@userId", new { userId }).ConfigureAwait(false))
                .Select(x => new { TenantId = TenantId, ActivityGroupName = x.Qualifier })
                .ToList();

            grants.Should().HaveCount(returnedGrants.Count());
            grants.Should().BeEquivalentTo(returnedGrants);
        }

        [Fact]
        public async Task ClearExistingCacheForUser()
        {
            // Arrange
            await cache.SetStringAsync(AuthorizationCacheKey.ForActivityGroups(), "{}").ConfigureAwait(false);
            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", new object[] { });
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncActivityGroups().ConfigureAwait(false);

            // Assert
            var cached = await cache.GetStringAsync(AuthorizationCacheKey.ForActivityGroups()).ConfigureAwait(false);
            cached.Should().BeNull();
        }

        [Fact]
        public async Task ThrowOperationFailed()
        {
            // Arrange
            var handler = MessageHandlerMother.NewHandler();
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            Func<Task> act = () => service.SyncActivityGroupGrants("userId");

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
