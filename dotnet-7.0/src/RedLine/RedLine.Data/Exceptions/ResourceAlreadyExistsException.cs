using System;
using System.Runtime.Serialization;

namespace RedLine.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt is made to create a resource that already exists.
    /// </summary>
    [Serializable]
    public class ResourceAlreadyExistsException : ResourceException
    {
        private static readonly int DefaultStatusCode = 412;

        private static readonly string DefaultMessage = "The specified resource already exists.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        /// <param name="message">A message that explains the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ResourceAlreadyExistsException(int statusCode, string resource, string identifier, string message, Exception innerException)
            : base(statusCode, resource, message ?? DefaultMessage, innerException)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ResourceAlreadyExistsException(string resource, string identifier, Exception innerException)
            : this(DefaultStatusCode, resource, identifier, $"{resource} with id {identifier} already exists.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        public ResourceAlreadyExistsException(string resource, string identifier)
            : this(resource, identifier, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected ResourceAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        public string Identifier { get; protected set; }
    }
}
