using System;
using System.Text.Json;

namespace RedLine.Data.Serialization
{
    /// <summary>
    /// Various serializer settings for using the <see cref="JsonSerializer" />.
    /// </summary>
    public static class RedLineJsonSerializerOptions
    {
        private static readonly Lazy<JsonSerializerOptions> DefaultSettings = new(() =>
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        /// <summary>
        /// The default serializer options.
        /// </summary>
        public static JsonSerializerOptions Default => DefaultSettings.Value;

        /// <summary>
        /// Serializer options specific to communicating with opa.
        /// </summary>
        public static JsonSerializerOptions Opa => DefaultSettings.Value;
    }
}
