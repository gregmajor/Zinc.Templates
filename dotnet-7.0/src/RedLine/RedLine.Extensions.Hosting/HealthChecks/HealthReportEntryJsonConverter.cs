using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RedLine.Extensions.Hosting.HealthChecks
{
    /// <summary>
    /// A <see cref="JsonConverter{T}"/> to serialize a <see cref="HealthReportEntry"/>.
    /// </summary>
    internal class HealthReportEntryJsonConverter : JsonConverter<HealthReportEntry>
    {
        /// <inheritdoc />
        public override HealthReportEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Not implemented
            return default;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, HealthReportEntry value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(options?.PropertyNamingPolicy?.ConvertName(nameof(value.Data)) ?? nameof(value.Data));
            JsonSerializer.Serialize(writer, value.Data, options);

            writer.WriteString(
                options?.PropertyNamingPolicy?.ConvertName(nameof(value.Description)) ?? nameof(value.Description),
                value.Description ?? value.Exception?.GetType().Name);

            writer.WriteString(
                options?.PropertyNamingPolicy?.ConvertName(nameof(value.Duration)) ?? nameof(value.Duration),
                value.Duration.ToString());

            writer.WriteString(
                options?.PropertyNamingPolicy?.ConvertName(nameof(value.Exception)) ?? nameof(value.Exception),
                value.Exception?.Message);

            writer.WriteString(
                options?.PropertyNamingPolicy?.ConvertName(nameof(value.Status)) ?? nameof(value.Status),
                value.Status.ToString());

            writer.WritePropertyName(options?.PropertyNamingPolicy?.ConvertName(nameof(value.Tags)) ?? nameof(value.Tags));
            JsonSerializer.Serialize(writer, value.Tags, options);

            writer.WriteEndObject();
        }
    }
}
