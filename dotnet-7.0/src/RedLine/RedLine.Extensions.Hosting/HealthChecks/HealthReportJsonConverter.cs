using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RedLine.Extensions.Hosting.HealthChecks
{
    /// <summary>
    /// A <see cref="JsonConverter{T}"/> to serialize a <see cref="HealthReport"/>.
    /// </summary>
    internal class HealthReportJsonConverter : JsonConverter<HealthReport>
    {
        /// <inheritdoc />
        public override HealthReport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Not implemented
            return default;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, HealthReport value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName(options?.PropertyNamingPolicy?.ConvertName(nameof(value.Entries)) ?? nameof(value.Entries));
            JsonSerializer.Serialize(writer, value.Entries, options);

            writer.WriteString(
                options?.PropertyNamingPolicy?.ConvertName(nameof(value.Status)) ?? nameof(value.Status),
                value.Status.ToString());

            writer.WriteString(
                options?.PropertyNamingPolicy?.ConvertName(nameof(value.TotalDuration)) ?? nameof(value.TotalDuration),
                value.TotalDuration.ToString());

            writer.WriteEndObject();
        }
    }
}
