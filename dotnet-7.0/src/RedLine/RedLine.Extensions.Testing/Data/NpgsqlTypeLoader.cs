using Npgsql;
using RedLine.Data;

namespace RedLine.Extensions.Testing.Data
{
    /// <summary>
    /// Service used to ensure an Npgsql connection pool loads composite types.
    /// </summary>
    public class NpgsqlTypeLoader
    {
        private readonly PostgresConnectionString connectionString;

        /// <summary>
        /// Initializes a new <see cref="NpgsqlTypeLoader"/>.
        /// </summary>
        /// <param name="connectionString">Injected connection string.</param>
        public NpgsqlTypeLoader(PostgresConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Reload types from the database.
        /// </summary>
        public void Reload()
        {
            using var connection = new NpgsqlConnection(connectionString.Value);
            connection.Open();
            connection.ReloadTypes();
            connection.Close();
        }
    }
}
