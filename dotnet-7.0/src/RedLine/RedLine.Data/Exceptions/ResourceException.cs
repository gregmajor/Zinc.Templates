using System;
using System.Runtime.Serialization;

namespace RedLine.Data.Exceptions
{
    /// <summary>
    /// An abstract base class for exceptions that occur when attempting to act upon application resources.
    /// </summary>
    [Serializable]
    public abstract class ResourceException : Domain.Exceptions.RedLineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="resource">The resource that was attempted to be acted upon.</param>
        /// <param name="message">A message that explains the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        protected ResourceException(
            int statusCode,
            string resource,
            string message,
            Exception innerException)
            : base(
                  statusCode,
                  message ?? $"Operation failed for resource {resource}.",
                  innerException)
        {
            Resource = resource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected ResourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the resource that was attempted to be modified.
        /// </summary>
        public string Resource { get; protected set; }
    }
}
