using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RedLine.Extensions.Hosting.HealthChecks
{
    internal static class HealthCheckJsonSerializerSettings
    {
        private static Lazy<JsonSerializerOptions> defaultSettings = new Lazy<JsonSerializerOptions>(() =>
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
            };

            options.Converters.Add(new HealthReportEntryJsonConverter());
            options.Converters.Add(new HealthReportJsonConverter());

            return options;
        });

        public static JsonSerializerOptions Default => defaultSettings.Value;
    }
}
