using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedLine.Domain.Services
{
    /// <inheritdoc />
    internal sealed class DataTypeConverter : JsonConverter<DataType>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, DataType value, JsonSerializer serializer)
        {
            if (!value.IsValidType())
            {
                throw new InvalidOperationException("Can't serialize invalid DataType.");
            }

            var token = JToken.FromObject((string)value);
            token.WriteTo(writer);
        }

        /// <inheritdoc />
        public override DataType ReadJson(JsonReader reader, Type objectType, DataType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;
            return DataType.Parse(value);
        }
    }
}
