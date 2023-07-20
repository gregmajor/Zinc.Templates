using System;
using System.Runtime.Serialization;

namespace RedLine.Domain.Exceptions
{
    /// <summary>
    /// The base exception class for all RedLine exceptions.
    /// </summary>
    [Serializable]
    public class RedLineException : Exception
    {
        private static readonly int DefaultStatusCode = 500;

        private static readonly string DefaultMessage = "An unexpected exception occurred.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RedLineException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RedLineException(int statusCode, string message, Exception innerException)
            : base(message ?? DefaultMessage, innerException)
        {
            if (statusCode < 100 || statusCode > 599)
            {
                throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, "The status code must be between 100 and 599.");
            }

            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedLineException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        public RedLineException(int statusCode, string message)
            : this(statusCode, message ?? DefaultMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedLineException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RedLineException(string message, Exception innerException)
            : this(DefaultStatusCode, message ?? DefaultMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedLineException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        public RedLineException(string message)
            : this(DefaultStatusCode, message ?? DefaultMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedLineException"/> class.
        /// </summary>
        public RedLineException()
            : this(DefaultStatusCode, DefaultMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedLineException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected RedLineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the exception status code.
        /// </summary>
        public int StatusCode { get; protected set; }
    }
}
