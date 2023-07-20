using Microsoft.Extensions.Configuration;

namespace RedLine.Extensions.Hosting.Web.Swagger
{
    internal class SwaggerConfiguration
    {
        private const string SectionName = "Swagger";

        /// <summary>
        /// Gets or sets a value indicating whether swagger is disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the AuthN authority.
        /// </summary>
        public string OAuth2Authority { get; set; }

        /// <summary>
        /// Gets or sets the redirect url for OAuth2 authentication used by swagger.
        /// </summary>
        public string OAuth2RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the client id for OAuth2 authentication used by swagger.
        /// </summary>
        public string ClientId { get; set; } = "swagger-api";

        /// <summary>
        /// Gets or sets the client name for OAuth2 authentication used by swagger.
        /// </summary>
        public string ClientName { get; set; } = "Swagger for API";

        /// <summary>
        /// Gets the Swagger configuration section.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        /// <returns><see cref="SwaggerConfiguration"/>.</returns>
        public static SwaggerConfiguration GetSwaggerConfiguration(IConfiguration configuration)
        {
            var config = configuration
                .GetSection(SectionName)
                .Get<SwaggerConfiguration>();

            return config;
        }
    }
}