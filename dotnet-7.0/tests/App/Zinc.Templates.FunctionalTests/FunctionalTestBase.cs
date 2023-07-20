using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.Data;
using Respawn;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests
{
    /// <summary>
    /// The base test class for all other functional tests.
    /// </summary>
    [Collection(nameof(FunctionalTestCollection))]
    public abstract class FunctionalTestBase : IAsyncLifetime
    {
        private readonly IServiceScope scope;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The test fixture that will be injected at runtime.</param>
        /// <param name="output">A helper class used to output debugging information in tests, injected at runtime if needed.</param>
        protected FunctionalTestBase(FunctionalTestFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
            SchemasToInclude = new[] { "app", "outbox", "authz" }; // TODO Add your schema(s) to delete between each test here
            scope = Fixture.ServiceProvider.CreateScope();
        }

        /// <summary>
        /// Gets the test fixture.
        /// </summary>
        protected FunctionalTestFixture Fixture { get; }

        /// <summary>
        /// Gets the <see cref="ITestOutputHelper"/> used to output debugging information in tests.
        /// </summary>
        protected ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Gets the schemas included in database clean up that runs after each test.
        /// </summary>
        protected string[] SchemasToInclude { get; set; }

        /// <summary>
        /// Gets the tenant identifier used for the tests.
        /// </summary>
        protected string TenantId { get; set; } = WellKnownId.TestTenant;

        /// <summary>
        /// A method used to retrieve a service from the container.
        /// </summary>
        /// <typeparam name="TService">The type of service to retrieve.</typeparam>
        /// <returns>An instance of <typeparamref name="TService"/>.</returns>
        protected TService GetRequiredService<TService>() => scope.ServiceProvider.GetRequiredService<TService>();

        /// <inheritdoc />
        async Task IAsyncLifetime.InitializeAsync()
        {
            var connectionString = GetRequiredService<PostgresConnectionString>().Value;

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

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            var cache = GetRequiredService<IDistributedCache>();
            await cache.RemoveAsync(AuthorizationCacheKey.ForGrants(GetRequiredService<IAuthenticationToken>().UserId)).ConfigureAwait(false);
            await cache.RemoveAsync(AuthorizationCacheKey.ForActivityGroups()).ConfigureAwait(false);
            scope.Dispose();
        }
    }
}
