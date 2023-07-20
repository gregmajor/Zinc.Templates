using System;
using RedLine.Domain.Events;

namespace RedLine.A3.Audit
{
    /// <summary>
    /// The interface that defines a contract for auditing activities.
    /// </summary>
    public interface IActivityAudited : IDomainEvent
    {
        /// <summary>
        /// Gets the name of the activity (command or query) that was executed.
        /// </summary>
        string ActivityName { get; set; }

        /// <summary>
        /// Gets the name of the application in which the activity was executed.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets the client IP address or endpoint name.
        /// </summary>
        string ClientAddress { get; set; }

        /// <summary>
        /// Gets a unique correlation identifier for the activity.
        /// </summary>
        Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets the exception thrown while executing the activity, or null if no exception was thrown.
        /// </summary>
        string Exception { get; set; }

        /// <summary>
        /// Gets the login of the user performing the activity, typically an email address.
        /// </summary>
        string Login { get; set; }

        /// <summary>
        /// Gets the activity request object (command or query) as a string, typically the JSON-serialized request object.
        /// </summary>
        string Request { get; set; }

        /// <summary>
        /// Gets the activity response object as a string, typically by calling response.ToString().
        /// </summary>
        string Response { get; set; }

        /// <summary>
        /// Gets the status code of the activity.
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        /// Gets the tenant for which the activity was executed.
        /// </summary>
        string TenantId { get; set; }

        /// <summary>
        /// Gets a time stamp of when the activity was executed.
        /// </summary>
        DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets the id of the user performing the activity.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets the name of the user performing the activity.
        /// </summary>
        string UserName { get; set; }
    }
}
