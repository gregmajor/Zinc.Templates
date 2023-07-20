using System.Collections.Generic;
using System.IO;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace RedLine.Data.Serialization
{
    /// <summary>
    /// Provides default <see cref="JsonSerializerSettings"/> to be used in RedLine.
    /// </summary>
    public static class RedLineNewtonsoftSerializerSettings
    {
        /// <summary>
        /// Gets settings to ignore <see cref="Stream"/>.
        /// </summary>
        public static JsonSerializerSettings IgnoreStream { get; } = new JsonSerializerSettings
        {
            ContractResolver = new IgnoreStreamContractResolver(),
        };

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        public static JsonSerializerSettings Default { get; } = ApplyDefaults(new JsonSerializerSettings());

        /// <summary>
        /// Applies the default <see cref="JsonSerializerSettings"/> settings.
        /// </summary>
        /// <param name="settings">The settings object to update.</param>
        /// <returns>The settings object.</returns>
        public static JsonSerializerSettings ApplyDefaults(JsonSerializerSettings settings)
        {
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            settings.ContractResolver = new PrivateSetterContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false,
                },
            };
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            settings.Converters = new List<JsonConverter> { new StringEnumConverter() };
            return settings;
        }
    }
}
