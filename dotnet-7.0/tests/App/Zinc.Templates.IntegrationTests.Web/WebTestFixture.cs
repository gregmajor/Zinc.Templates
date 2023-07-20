using System;
using Alba;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RedLine.Data;
using RedLine.Extensions.Testing.Data;
using Xunit.Abstractions;
using Zinc.Templates.Data.Migrations;

namespace Zinc.Templates.IntegrationTests.Web
{
    public class WebTestFixture : IDisposable
    {
        private readonly TestOutputSink sink = new TestOutputSink();

        /// <summary>
        /// This constructor initializes tests for running web scenarios with a database.
        /// </summary>
        public WebTestFixture()
        {
            // run migrations
            var services = Startup.CreateServiceProviderForMigrations();
            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<Migrator>().RunMigrations();
            }

            // create test host
            TestHost = new AlbaHost(Startup.CreateTestHostBuilder(sink));
            TestHost.Services.GetRequiredService<NpgsqlTypeLoader>().Reload();
            ConnectionString = TestHost.Services.GetRequiredService<PostgresConnectionString>();

            ScopeFactory = TestHost.Services.GetRequiredService<IServiceScopeFactory>();
        }

        public PostgresConnectionString ConnectionString { get; }

        public IAlbaHost TestHost { get; }

        public IServiceScopeFactory ScopeFactory { get; }

        public void RegisterTestOutputHelper(ITestOutputHelper output)
        {
            sink.Register(output);
        }

        public void UnregisterTestOutputHelper()
        {
            sink.Unregister();
        }

        /// <summary>
        /// This method destroys the database created in the constructor after all tests have run.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Destroy the database...
                var databaseName = ConnectionString.DatabaseName;

                using (var connection = new NpgsqlConnection(ConnectionString.ServerConnectionString))
                {
                    connection.Execute($@"
                        SELECT pg_terminate_backend(pg_stat_activity.pid)
                        FROM pg_stat_activity
                        WHERE pg_stat_activity.datname = '{databaseName}';");

                    connection.Execute($"DROP DATABASE \"{databaseName}\";");
                }

                TestHost?.Dispose();
            }
        }
    }
}

