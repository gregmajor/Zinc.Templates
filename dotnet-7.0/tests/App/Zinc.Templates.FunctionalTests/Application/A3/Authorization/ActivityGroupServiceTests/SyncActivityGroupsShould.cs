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
using RedLine.Application.A3.Authorization;
using RedLine.Data.Exceptions;
using RedLine.Domain;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.FunctionalTests.Mothers;
using static Zinc.Templates.FunctionalTests.Mothers.MessageHandlerMother;

namespace Zinc.Templates.FunctionalTests.Application.A3.Authorization.ActivityGroupServiceTests
{
    public class SyncActivityGroupsShould : FunctionalTestBase
    {
        private readonly IDistributedCache cache;
        private readonly IDbConnection connection;
        private readonly ICorrelationId correlationId;
        private readonly IAuthenticationToken token;
        private readonly ILogger<ActivityGroupService> logger;

        public SyncActivityGroupsShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            this.cache = GetRequiredService<IDistributedCache>();
            this.connection = GetRequiredService<IDbConnection>();
            this.correlationId = GetRequiredService<ICorrelationId>();
            this.token = GetRequiredService<IAuthenticationToken>();
            this.logger = GetRequiredService<ILogger<ActivityGroupService>>();
        }

        [Fact]
        public async Task PersistActivityGroups()
        {
            // Arrange
            var activityGroups = new[]
            {
                new { Name = "ActGroup1", TenantId = TenantId, Activities = new[] { new { ActivityName = "Activity1" } } },
            };
            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", activityGroups);
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncActivityGroups().ConfigureAwait(false);

            // Assert
            var groups = await connection.QueryAsync<string>("select distinct name from authz.activity_group").ConfigureAwait(false);
            groups.Should().HaveCount(activityGroups.Count());
            groups.First().Should().Be(activityGroups[0].Name);
        }

        [Fact]
        public async Task ReplaceExistingActivityGroups()
        {
            // Arrange
            await connection.ExecuteAsync("insert into authz.activity_group(tenant_id, name, activity_name) values(@TenantId, 'ActGroup2', 'Activity1')", new { TenantId }).ConfigureAwait(false);
            var activityGroups = new[]
            {
                new { Name = "ActGroup1", TenantId = TenantId, Activities = new[] { new { ActivityName = "Activity1" } } },
            };
            var handler = MessageHandlerMother.NewHandler().WithSuccessResponse("host.docker.internal", activityGroups);
            var service = new ActivityGroupService(GetHttpClient(handler), correlationId, connection, token, logger, cache);

            // Act
            await service.SyncActivityGroups().ConfigureAwait(false);

            // Assert
            var groups = await connection.QueryAsync<string>("select distinct name from authz.activity_group").ConfigureAwait(false);
            groups.Should().HaveCount(activityGroups.Count());
            groups.First().Should().Be(activityGroups[0].Name);
        }

        [Fact]
        public async Task ClearExistingCache()
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
            Func<Task> act = () => service.SyncActivityGroups();

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
