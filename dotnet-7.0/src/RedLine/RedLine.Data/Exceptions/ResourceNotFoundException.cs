using System;
using System.Runtime.Serialization;

namespace RedLine.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown if a resource is not found.
    /// </summary>
    [Serializable]
    public class ResourceNotFoundException : ResourceException
    {
        private static readonly int DefaultStatusCode = 404;

        private static readonly string DefaultMessage = "The specified resource was not found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        /// <param name="message">A message that explains the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ResourceNotFoundException(int statusCode, string resource, string identifier, string message, Exception innerException)
            : base(statusCode, resource, message ?? DefaultMessage, innerException)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ResourceNotFoundException(string resource, string identifier, Exception innerException)
            : this(DefaultStatusCode, resource, identifier, $"{resource} with id {identifier} was not found.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="identifier">The resource unique identifier.</param>
        public ResourceNotFoundException(string resource, string identifier)
            : this(resource, identifier, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of the resource.
        /// </summary>
        public string Identifier { get; protected set; }
    }
}
