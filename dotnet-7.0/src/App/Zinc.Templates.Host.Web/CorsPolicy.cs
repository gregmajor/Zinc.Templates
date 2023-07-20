using System.Collections.Generic;

namespace Zinc.Templates.Host.Web
{
    /// <summary>
    /// CORS configuration settings.
    /// </summary>
    internal class CorsPolicy
    {
        /// <summary>
        /// Gets the CorsPolicy config section name.
        /// </summary>
        public static readonly string SectionName = nameof(CorsPolicy);

        /// <summary>
        /// The origins allowed to call the api.
        /// </summary>
        public List<string> AllowedOrigins { get; set; } = new List<string>();

        /// <summary>
        /// The non-default headers that should be exposed to the client.
        /// </summary>
        public List<string> ExposedHeaders { get; set; } = new List<string>();
    }
}
