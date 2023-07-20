using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Alba;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Npgsql;
using RedLine.A3.Authorization;
using RedLine.Data.Serialization;
using RedLine.Domain;
using Respawn;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.IntegrationTests.Web.Mothers;

namespace Zinc.Templates.IntegrationTests.Web
{
    [Collection(nameof(WebTestCollection))]
    public abstract class WebTestBase : IAsyncLifetime
    {
        private readonly List<string> cacheKeys = new();

        protected WebTestBase(WebTestFixture fixture, ITestOutputHelper output)
        {
            fixture.RegisterTestOutputHelper(output);
            Fixture = fixture;
            Output = output;
            SchemasToInclude = new[] { "app", "outbox", "authz" }; // TODO Add your schema(s) to delete between each test here
            Scope = Fixture.ScopeFactory.CreateScope();
        }

        protected static string TenantId => WellKnownIds.TenantId;

        /// <summary>
        /// The system to test scenarios against.
        /// </summary>
        protected IAlbaHost TestHost => Fixture.TestHost;

        /// <summary>
        /// Gets the test fixture.
        /// </summary>
        protected WebTestFixture Fixture { get; }

        /// <summary>
        /// Gets the <see cref="ITestOutputHelper"/> used to output debugging information in tests.
        /// </summary>
        protected ITestOutputHelper Output { get; }

        /// <summary>
        /// Gets the schemas included in database clean up that runs after each test.
        /// </summary>
        protected string[] SchemasToInclude { get; }

        protected IServiceScope Scope { get; }

        protected TService GetRequiredService<TService>() => Scope.ServiceProvider.GetRequiredService<TService>();

        public Task<IScenarioResult> AuthorizedScenario(Action<Scenario> testScenario) => AuthorizedScenario(TestUser.GetTestuser1(), testScenario);

        public async Task<IScenarioResult> AuthorizedScenario(TestUser user, Action<Scenario> testScenario)
        {
            await SetAuthorizationCache(user).ConfigureAwait(false);

            var bearerToken = IdentityMother.GenerateJWTToken(user);
            var correlationId = Guid.NewGuid();

            return await TestHost.Scenario(_ =>
            {
                _.WithRequestHeader(HeaderNames.Authorization, $"Bearer {bearerToken}");
                _.WithRequestHeader(RedLineHeaderNames.CorrelationId, correlationId.ToString());

                testScenario(_);
            }).ConfigureAwait(false);
        }

        public Task<IScenarioResult> AuthenticatedScenario(Action<Scenario> testScenario) => AuthorizedScenario(TestUser.GetTestuser1().WithoutGrants(), testScenario);

        public Task<IScenarioResult> AnonymousScenario(Action<Scenario> testScenario) => TestHost.Scenario(_ => testScenario(_));

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await ResetCache().ConfigureAwait(false);
            Fixture.UnregisterTestOutputHelper();
            Scope.Dispose();
        }

        /// <inheritdoc />
        async Task IAsyncLifetime.InitializeAsync()
        {
            await ResetDatabase().ConfigureAwait(false);
        }

        private async Task SetAuthorizationCache(TestUser user)
        {
            var grants = user.Grants;
            if (grants != null)
            {
                await SetCache(AuthorizationCacheKey.ForGrants(IdentityMother.Upn(user)), grants).ConfigureAwait(false);
            }
        }

        private Task SetCache(string key, object obj)
        {
            cacheKeys.Add(key);
            return GetRequiredService<IDistributedCache>().SetStringAsync(key, JsonSerializer.Serialize(obj, RedLineJsonSerializerOptions.Opa));
        }

        private async Task ResetCache()
        {
            var cache = GetRequiredService<IDistributedCache>();
            foreach (var key in cacheKeys)
            {
                await cache.RemoveAsync(key).ConfigureAwait(false);
            }
        }

        private async Task ResetDatabase()
        {
            var connectionString = Fixture.ConnectionString.Value;

            var checkpoint = new Checkpoint
            {
                SchemasToInclude = SchemasToInclude,
                DbAdapter = DbAdapter.Postgres,
            };

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                await checkpoint.Reset(connection).ConfigureAwait(false);
            }
        }
    }
}
