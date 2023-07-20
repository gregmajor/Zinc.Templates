using Microsoft.Extensions.Configuration;
using Npgsql;
using RedLine.Domain;

namespace RedLine.Data
{
    /// <summary>
    /// Provides access to the application's Postgres connection string.
    /// </summary>
    public sealed class PostgresConnectionString : IConnectionString
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public PostgresConnectionString(IConfiguration configuration)
        {
            Value = ApplicationContext.Expand(configuration.GetConnectionString(Name));
        }

        /// <inheritdoc/>
        public string Name => ConnectionStringNames.Postgres;

        /// <inheritdoc/>
        public string Value { get; }

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public string DatabaseName => Builder().Database;

        /// <summary>
        /// Gets the connection string to the server without a specified database.
        /// </summary>
        public string ServerConnectionString
        {
            get
            {
                var builder = Builder();
                builder.Database = "postgres";
                return builder.ToString();
            }
        }

        private NpgsqlConnectionStringBuilder Builder() => new NpgsqlConnectionStringBuilder(Value);
    }
}
