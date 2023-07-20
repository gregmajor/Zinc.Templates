using System;

namespace RedLine.Domain
{
    /// <summary>
    /// The interface that defines a contract for the request correlation identifier.
    /// </summary>
    public interface ICorrelationId
    {
        /// <summary>
        /// Gets or sets the request correlation identifier.
        /// </summary>
        Guid Value { get; set; }
    }

    /// <summary>
    /// An injectable correlation id, provided by the client as an http header and registered in the container by the hosting process.
    /// </summary>
    public sealed class CorrelationId : ICorrelationId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationId"/> class.
        /// </summary>
        /// <param name="value">The value of the correlation id.</param>
        public CorrelationId(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Constructs a CorrelationId from a string.
        /// </summary>
        /// <param name="value">The value of the correlation id as a string.</param>
        public CorrelationId(string value)
        {
            Value = Guid.Parse(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationId"/> class.
        /// </summary>
        public CorrelationId()
        {
            Value = Guid.NewGuid();
        }

        /// <inheritdoc/>
        public Guid Value { get; set; }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}
