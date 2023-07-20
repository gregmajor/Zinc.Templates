using System;
using System.Runtime.Serialization;

namespace RedLine.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a concurrency conflict occurs.
    /// </summary>
    [Serializable]
    public class ConcurrencyException : ResourceException
    {
        private static readonly int DefaultStatusCode = 412;

        private static readonly string DefaultMessage = "A concurrency conflict occurred.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        /// <param name="message">A message that explains the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConcurrencyException(string resource, string identifier, string message, Exception innerException)
            : base(DefaultStatusCode, resource, message ?? DefaultMessage, innerException)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        public ConcurrencyException(string resource, string identifier)
            : this(resource, identifier, $"A concurrency conflict occurred while saving {resource} with id {identifier}.", null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        public string Identifier { get; protected set; }
    }
}
