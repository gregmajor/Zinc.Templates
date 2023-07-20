using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;
using Npgsql;
using RedLine.Data;

namespace Zinc.Templates.Data.Migrations
{
    /// <summary>
    /// The class that is responsible for running data migrations.
    /// </summary>
    public class Migrator
    {
        private const string LogMessageTemplate = "[MIGRATIONS]==> {Message}";
        private const int LockId = -1023948;

        /// <summary>
        /// Initializes a new instance of the <see cref="Migrator"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="logger">A diagnostic logger.</param>
        /// <param name="migrationRunner">The migration runner.</param>
        public Migrator(
            PostgresConnectionString connectionString,
            ILogger<Migrator> logger,
            IMigrationRunner migrationRunner)
        {
            ConnectionString = connectionString;
            Logger = logger;
            MigrationRunner = migrationRunner;
        }

        /// <summary>
        /// Gets the database connectionstring.
        /// </summary>
        protected PostgresConnectionString ConnectionString { get; }

        /// <summary>
        /// Gets a diagnostic logger that can be used for debugging.
        /// </summary>
        protected ILogger<Migrator> Logger { get; }

        /// <summary>
        /// Gets the <see cref="IMigrationRunner"/> that will run the migrations.
        /// </summary>
        protected IMigrationRunner MigrationRunner { get; }

        /// <summary>
        /// Runs all new migrations.
        /// </summary>
        public void RunMigrations()
        {
            CreateDatabase();

            Logger.LogInformation(LogMessageTemplate, "Attempting to acquire advisory lock...");

            using (new AdvisoryLock(ConnectionString, LockId))
            {
                Logger.LogInformation(LogMessageTemplate, "Advisory lock acquired.");

                MigrationRunner.MigrateUp();

                Logger.LogInformation(LogMessageTemplate, "Migrations completed successfully!");
            }

            Logger.LogInformation(LogMessageTemplate, "Advisory lock released.");
        }

        private void CreateDatabase()
        {
            var databaseName = ConnectionString.DatabaseName;

            Logger.LogInformation(LogMessageTemplate, $"Attempting to create database '{databaseName}'...");

            using (var connection = new NpgsqlConnection(ConnectionString.ServerConnectionString))
            {
                connection.Open();

                if (DatabaseExists(connection, databaseName))
                {
                    return;
                }

                connection.Execute($"CREATE DATABASE \"{databaseName}\"");

                Logger.LogInformation(LogMessageTemplate, $"'{databaseName}' was created successfully.");
            }
        }

        private bool DatabaseExists(NpgsqlConnection connection, string databaseName)
        {
            const string sql = "SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @DatabaseName);";

            return connection.ExecuteScalar<bool>(sql, new { DatabaseName = databaseName });
        }
    }
}
