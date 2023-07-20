using System;
using System.Runtime.Serialization;
using RedLine.A3;
using RedLine.A3.Authentication;
using RedLine.Domain.Exceptions;

namespace RedLine.Application.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a user is not authorized to perform an operation.
    /// </summary>
    [Serializable]
    public class NotAuthorizedException : RedLineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the exception, which should be 401 or 403.</param>
        /// <param name="user">The current user's <see cref="IAccessToken"/>.</param>
        /// <param name="message">The message that explains why the exception occurred.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        protected internal NotAuthorizedException(
            int statusCode,
            IAuthenticationToken user,
            string message,
            Exception inner)
            : base(statusCode, message, inner)
        {
            Login = user.Login;
            UserId = user.UserId;
            UserName = user.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the exception, which should be 401 or 403.</param>
        /// <param name="user">The current user's <see cref="IAccessToken"/>.</param>
        /// <param name="message">The message that explains why the exception occurred.</param>
        protected internal NotAuthorizedException(int statusCode, IAuthenticationToken user, string message)
            : this(statusCode, user, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotAuthorizedException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected NotAuthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets or sets the user login that was not authorized.
        /// </summary>
        public string Login { get; protected set; }

        /// <summary>
        /// Gets or sets the user that was not authorized.
        /// </summary>
        public string UserId { get; protected set; }

        /// <summary>
        /// Gets or sets the user name that was not authorized.
        /// </summary>
        public string UserName { get; protected set; }

        /// <summary>
        /// A factory method used to create an <see cref="NotAuthorizedException"/> with a 401 status code.
        /// </summary>
        /// <param name="inner">The exception that is the cause of this exception.</param>
        /// <returns><see cref="NotAuthorizedException"/>.</returns>
        public static NotAuthorizedException BecauseNotAuthenticated(Exception inner)
        {
            return new NotAuthorizedException(401, AuthenticationToken.Anonymous, "Anonymous access is not allowed.", inner);
        }

        /// <summary>
        /// A factory method used to create an <see cref="NotAuthorizedException"/> with a 401 status code.
        /// </summary>
        /// <returns><see cref="NotAuthorizedException"/>.</returns>
        public static NotAuthorizedException BecauseNotAuthenticated() => BecauseNotAuthenticated(null);

        /// <summary>
        /// A factory method used to create an <see cref="NotAuthorizedException"/> with a 403 status code.
        /// </summary>
        /// <param name="user">The current user's <see cref="IAuthenticationToken"/>.</param>
        /// <param name="activityName">The name of the activity being executed.</param>
        /// <param name="resourceId">The resource being accessed, if any.</param>
        /// <param name="inner">The exception that is the cause of this exception.</param>
        /// <returns><see cref="NotAuthorizedException"/>.</returns>
        public static NotAuthorizedException BecauseForbidden(IAuthenticationToken user, string activityName, string resourceId, Exception inner)
        {
            var clarification = string.IsNullOrEmpty(resourceId) ? string.Empty : $" on {resourceId}";
            return new NotAuthorizedException(403, user, $"{user.FullName} is not authorized for {activityName}{clarification}.", inner);
        }

        /// <summary>
        /// A factory method used to create an <see cref="NotAuthorizedException"/> with a 403 status code.
        /// </summary>
        /// <param name="user">The current user's <see cref="IAuthenticationToken"/>.</param>
        /// <param name="activityName">The name of the activity being executed.</param>
        /// <param name="resourceId">The resource being accessed, if any.</param>
        /// <returns><see cref="NotAuthorizedException"/>.</returns>
        public static NotAuthorizedException BecauseForbidden(IAuthenticationToken user, string activityName, string resourceId) => BecauseForbidden(user, activityName, resourceId, null);

        /// <summary>
        /// A factory method used to create an <see cref="NotAuthorizedException"/> with a 403 status code.
        /// </summary>
        /// <param name="user">The current user's <see cref="IAuthenticationToken"/>.</param>
        /// <param name="message">A message that explains the exception.</param>
        /// <returns><see cref="NotAuthorizedException"/>.</returns>
        public static NotAuthorizedException Because(IAuthenticationToken user, string message) => new NotAuthorizedException(403, user, message, null);
    }
}
