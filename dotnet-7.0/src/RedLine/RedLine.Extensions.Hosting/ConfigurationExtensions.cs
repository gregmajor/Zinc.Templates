using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using RedLine.Domain;

namespace RedLine.Extensions.Hosting
{
    /// <summary>
    /// Provides extension methods for using configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the config settings to initialize the <see cref="ApplicationContext"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration" />.</param>
        /// <returns>The dictionary used to initialize the <see cref="ApplicationContext"/>.</returns>
        public static IDictionary<string, string> GetApplicationContextConfiguration(this IConfiguration configuration)
        {
            return configuration
                .GetSection(nameof(ApplicationContext))
                .GetChildren()
                .ToDictionary(c => c.Key, c => c.Value);
        }
    }
}
