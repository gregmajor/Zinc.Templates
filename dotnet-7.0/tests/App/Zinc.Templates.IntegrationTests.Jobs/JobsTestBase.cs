using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Alba;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RedLine.A3.Authorization;
using RedLine.Data.Serialization;
using RedLine.Domain;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.IntegrationTests.Jobs
{
    [Collection(nameof(JobsTestCollection))]
    public abstract class JobsTestBase : IAsyncLifetime
    {
        protected JobsTestBase(JobsTestFixture fixture, ITestOutputHelper output)
        {
            fixture.RegisterTestOutputHelper(output);
            Fixture = fixture;
            Output = output;
            SchemasToInclude = new[] { "app", "outbox", "authz" }; // TODO Add your schema(s) to delete between each test here
            Scope = TestHost.Services.CreateScope();
        }

        /// <summary>
        /// The system to test scenarios against.
        /// </summary>
        protected IAlbaHost TestHost => Fixture.TestHost;

        /// <summary>
        /// Gets the test fixture.
        /// </summary>
        protected JobsTestFixture Fixture { get; }

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

        public Task<IScenarioResult> AuthorizedScenario(Action<Scenario> testScenario) => throw new NotImplementedException();

        public Task<IScenarioResult> AnonymousScenario(Action<Scenario> testScenario) => TestHost.Scenario(_ => testScenario(_));

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await ClearServiceAccountGrants().ConfigureAwait(false);
            Fixture.UnregisterTestOutputHelper();
            Scope.Dispose();
        }

        /// <inheritdoc />
        async Task IAsyncLifetime.InitializeAsync()
        {
            await ResetDatabase().ConfigureAwait(false);
            await SetServiceAccountGrants().ConfigureAwait(false);
        }

        private Task SetServiceAccountGrants()
        {
            var grants = new Dictionary<string, object>
            {
                { "*:*:*", new { ExpiresOn = (long?)null } },
            };

            return GetRequiredService<IDistributedCache>().SetStringAsync(AuthorizationCacheKey.ForGrants(ApplicationContext.ServiceAccountName), JsonSerializer.Serialize(grants, RedLineJsonSerializerOptions.Opa));
        }

        private async Task ClearServiceAccountGrants()
        {
            var cache = GetRequiredService<IDistributedCache>();
            await cache.RemoveAsync(AuthorizationCacheKey.ForGrants(ApplicationContext.ServiceAccountName)).ConfigureAwait(false);
            await cache.RemoveAsync(AuthorizationCacheKey.ForActivityGroups()).ConfigureAwait(false);
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
