using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RedLine.Data.Outbox.Serialization
{
    /// <summary>
    /// An <see cref="JsonConverter{T}"/> used to convert <see cref="OutboxMessage"/>s.
    /// </summary>
    internal class OutboxMessageJsonConverter : JsonConverter<OutboxMessage>
    {
        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(OutboxMessage).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public override OutboxMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Ensure(reader, JsonTokenType.StartObject);

            var outboxMessage = new OutboxMessage();

            Type messageBodyType = typeof(object);

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                Ensure(reader, JsonTokenType.PropertyName);
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "$messageBodyType":
                        messageBodyType = Type.GetType(reader.GetString());
                        break;
                    case nameof(outboxMessage.MessageHeaders):
                        outboxMessage.MessageHeaders = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
                        break;
                    case nameof(outboxMessage.MessageId):
                        outboxMessage.MessageId = reader.GetString();
                        break;
                    case nameof(outboxMessage.MessageBody):
                        // Reading as JsonElement for now, since the message type is not read yet.
                        outboxMessage.MessageBody = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                        break;
                    case nameof(outboxMessage.Destination):
                        outboxMessage.Destination = reader.GetString();
                        break;
                    default:
                        throw new JsonException($"Unexpected property name {propertyName}.");
                }
            }

            // Now that we have message type, we can set MessageBody to be of correct type.
            outboxMessage.MessageBody = JsonSerializer.Deserialize(
                outboxMessage.MessageBody.ToString(),
                messageBodyType,
                options);

            return outboxMessage;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, OutboxMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("$messageBodyType");
            var messageBodyType = value.MessageBody.GetType();
            var fullNameAndAssembly = $"{messageBodyType.FullName}, {messageBodyType.Assembly.GetName().Name}";
            JsonSerializer.Serialize(writer, fullNameAndAssembly);

            writer.WriteString(nameof(value.MessageId), value.MessageId);
            writer.WriteString(nameof(value.Destination), value.Destination);

            writer.WritePropertyName(nameof(value.MessageHeaders));
            JsonSerializer.Serialize(writer, value.MessageHeaders, options);

            writer.WritePropertyName(nameof(value.MessageBody));
            JsonSerializer.Serialize(writer, value.MessageBody, value.MessageBody.GetType(), options);

            writer.WriteEndObject();
        }

        private void Ensure(Utf8JsonReader reader, JsonTokenType expected)
        {
            if (reader.TokenType != expected)
            {
                throw new JsonException($"Expected {Enum.GetName(typeof(JsonTokenType), expected)}, but found {reader.TokenType} at {reader.Position}.");
            }
        }
    }
}
