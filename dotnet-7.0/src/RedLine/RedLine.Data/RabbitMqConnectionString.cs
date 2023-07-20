using Microsoft.Extensions.Configuration;
using RedLine.Domain;

namespace RedLine.Data
{
    /// <summary>
    /// Provides access to the application's Postgres connection string.
    /// </summary>
    public sealed class RabbitMqConnectionString : IConnectionString
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public RabbitMqConnectionString(IConfiguration configuration)
        {
            Value = ApplicationContext.Expand(configuration.GetConnectionString(Name));
        }

        /// <inheritdoc/>
        public string Name => ConnectionStringNames.RabbitMQ;

        /// <inheritdoc/>
        public string Value { get; }
    }
}
