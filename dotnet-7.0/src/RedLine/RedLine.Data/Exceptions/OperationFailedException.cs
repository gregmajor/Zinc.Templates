using System;
using System.Runtime.Serialization;

namespace RedLine.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a data layer operation fails.
    /// </summary>
    [Serializable]
    public class OperationFailedException : ResourceException
    {
        private static readonly int DefaultStatusCode = 500;

        private static readonly string DefaultMessage = "The operation failed.";

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFailedException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="operation">The operation that was executed.</param>
        /// <param name="message">A message that explains the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public OperationFailedException(int statusCode, string resource, string operation, string message, Exception innerException)
            : base(statusCode, resource, message ?? DefaultMessage, innerException)
        {
            Operation = operation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFailedException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="operation">The operation that was executed.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public OperationFailedException(string resource, string operation, Exception innerException)
            : this(DefaultStatusCode, resource, operation, $"{operation} failed for resource {resource}.", innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFailedException"/> class.
        /// </summary>
        /// <param name="resource">The resource that was attempted to be modified.</param>
        /// <param name="operation">The operation that was executed.</param>
        public OperationFailedException(string resource, string operation)
            : this(resource, operation, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationFailedException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected OperationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the operation that failed.
        /// </summary>
        public string Operation { get; protected set; }
    }
}
