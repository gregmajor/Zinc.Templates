using System;
using System.Runtime.Serialization;

namespace RedLine.Domain.Exceptions
{
    /// <summary>
    /// The base exception class of the domain layer.
    /// </summary>
    [Serializable]
    public class DomainException : RedLineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DomainException(int statusCode, string message, Exception innerException)
            : base(statusCode, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        public DomainException(int statusCode, string message)
            : base(statusCode, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        public DomainException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        public DomainException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
