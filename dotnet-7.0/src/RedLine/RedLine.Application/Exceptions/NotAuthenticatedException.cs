using System;
using System.Runtime.Serialization;
using RedLine.Domain.Exceptions;

namespace RedLine.Application.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a user cannot be authenticated.
    /// </summary>
    [Serializable]
    public class NotAuthenticatedException : RedLineException
    {
        [NonSerialized]
        private static readonly int DefaultStatusCode = 401;

        [NonSerialized]
        private static readonly string DefaultMessage = "Authentication failed.";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public NotAuthenticatedException(int statusCode, string message, Exception innerException)
            : base(statusCode, message ?? DefaultMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        public NotAuthenticatedException(int statusCode, string message)
            : base(statusCode, message ?? DefaultMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        public NotAuthenticatedException(int statusCode)
            : base(statusCode, DefaultMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public NotAuthenticatedException()
            : base(DefaultStatusCode, DefaultMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthenticatedException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected NotAuthenticatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// A factory method used to create an <see cref="NotAuthenticatedException"/> with a 401 status code.
        /// </summary>
        /// <param name="claim">The claim that was missing from the bearer token.</param>
        /// <returns><see cref="NotAuthenticatedException"/>.</returns>
        public static NotAuthenticatedException BecauseMissingClaim(string claim)
        {
            return new NotAuthenticatedException(401, $"Authentication failed because the '{claim}' claim was missing.");
        }
    }
}
