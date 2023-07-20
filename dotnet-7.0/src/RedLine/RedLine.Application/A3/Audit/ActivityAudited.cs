using System;
using RedLine.A3.Audit;

// IMPORTANT: DO NOT CHANGE THIS NAMESPACE
namespace Krypton.Audit
{
    /// <summary>
    /// This class is the NServiceBus message that we publish. We need this class because the
    /// ActivityAudit class has generic TRequest/TResponse parameters, and it's best not to use
    /// generic messages with NServiceBus...hence this class.
    /// </summary>
    public class ActivityAudited : IActivityAudited
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="audit">The activity audit containing the details for this message.</param>
        public ActivityAudited(IActivityAudited audit)
        {
            ActivityName = audit.ActivityName;
            ApplicationName = audit.ApplicationName;
            ClientAddress = audit.ClientAddress;
            CorrelationId = audit.CorrelationId;
            Exception = audit.Exception;
            Login = audit.Login;
            Request = audit.Request;
            Response = audit.Response;
            StatusCode = audit.StatusCode;
            TenantId = audit.TenantId;
            Timestamp = audit.Timestamp;
            UserId = audit.UserId;
            UserName = audit.UserName;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <remarks>This ctor is needed to deserialize the event from the outbox.</remarks>
        public ActivityAudited()
        { }

        /// <inheritdoc />
        public string ActivityName { get; set; }

        /// <inheritdoc />
        public string ApplicationName { get; set; }

        /// <inheritdoc />
        public string ClientAddress { get; set; }

        /// <inheritdoc />
        public Guid CorrelationId { get; set; }

        /// <inheritdoc />
        public string Exception { get; set; }

        /// <inheritdoc />
        public string Login { get; set; }

        /// <inheritdoc />
        public string Request { get; set; }

        /// <inheritdoc />
        public string Response { get; set; }

        /// <inheritdoc />
        public int StatusCode { get; set; }

        /// <inheritdoc />
        public string TenantId { get; set; }

        /// <inheritdoc />
        public DateTime Timestamp { get; set; }

        /// <inheritdoc />
        public string UserId { get; set; }

        /// <inheritdoc />
        public string UserName { get; set; }
    }
}
