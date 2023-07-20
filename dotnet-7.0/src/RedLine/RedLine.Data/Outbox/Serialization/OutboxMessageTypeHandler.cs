using System;
using System.Data;
using System.Text.Json;

namespace RedLine.Data.Outbox.Serialization
{
    /// <summary>
    /// A custom <see cref="Dapper.SqlMapper.ITypeHandler"/> for <see cref="OutboxMessage"/>s.
    /// </summary>
    internal class OutboxMessageTypeHandler : Dapper.SqlMapper.ITypeHandler
    {
        private readonly JsonSerializerOptions serializerOptions;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/> to use.</param>
        public OutboxMessageTypeHandler(JsonSerializerOptions serializerOptions)
        {
            this.serializerOptions = serializerOptions;
        }

        /// <inheritdoc />
        public object Parse(Type destinationType, object value)
        {
            return (value == null)
                ? (object)DBNull.Value
                : JsonSerializer.Deserialize(value.ToString(), destinationType, serializerOptions);
        }

        /// <inheritdoc />
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.DbType = DbType.AnsiString;
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : JsonSerializer.Serialize(value, serializerOptions);
        }
    }
}
