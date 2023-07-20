using System;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RedLine.Data;
using RedLine.Extensions.Testing.Data;
using Zinc.Templates.Data.Migrations;

namespace Zinc.Templates.FunctionalTests
{
    /// <summary>
    /// This class sets up all the necessary services, such as database, configuration, mocking unneeded
    /// services, etc., in order to create an environment in which tests can run.
    /// </summary>
    public class FunctionalTestFixture : IDisposable
    {
        /// <summary>
        /// This constructor initializes the configuration settings in the appsettings.test.json file in the
        /// root of the project. It also registers the ServiceCollection extensions in the Application layer
        /// and creates the database in which tests will run.
        /// </summary>
        public FunctionalTestFixture()
        {
            ServiceProvider = Startup.ConfigureServices();

            using var scope = ServiceProvider.CreateScope();

            scope
                .ServiceProvider
                .GetRequiredService<Migrator>()
                .RunMigrations();

            scope
                .ServiceProvider
                .GetRequiredService<NpgsqlTypeLoader>()
                .Reload();
        }

        /// <summary>
        /// This is a reference to the ServiceProvider in case a service needs to be retrieved that is not provided by the fixture.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

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
                var connectionString = ServiceProvider.GetRequiredService<PostgresConnectionString>();
                var databaseName = connectionString.DatabaseName;

                using (var connection = new NpgsqlConnection(connectionString.ServerConnectionString))
                {
                    connection.Execute($@"
                        SELECT pg_terminate_backend(pg_stat_activity.pid)
                        FROM pg_stat_activity
                        WHERE pg_stat_activity.datname = '{databaseName}';");

                    connection.Execute($"DROP DATABASE \"{databaseName}\";");
                }
            }
        }
    }
}
