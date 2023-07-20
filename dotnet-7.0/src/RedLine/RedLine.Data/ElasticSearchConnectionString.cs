using Microsoft.Extensions.Configuration;
using RedLine.Domain;

namespace RedLine.Data
{
    /// <summary>
    /// Provides access to the application's Elasticsearch connection string.
    /// </summary>
    public sealed class ElasticSearchConnectionString : IConnectionString
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public ElasticSearchConnectionString(IConfiguration configuration)
        {
            Value = ApplicationContext.Expand(configuration.GetConnectionString(Name));
        }

        /// <inheritdoc/>
        public string Name => ConnectionStringNames.ElasticSearch;

        /// <inheritdoc/>
        public string Value { get; }
    }
}
