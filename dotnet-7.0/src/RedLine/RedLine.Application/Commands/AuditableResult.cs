namespace RedLine.Application.Commands
{
    /// <summary>
    /// A command response that better facilitates auditing.
    /// </summary>
    /// <typeparam name="TResult">The response type.</typeparam>
    /// <remarks>
    /// This class is useful to customize what is seen in the Audit Log. When a request goes
    /// through the MediatR, the AuditBehavior collects details about the request and response.
    /// For the response, the behavior just calls response.ToString(), which by default returns
    /// the class name of the response. If, however, you want something useful for the Audit Log,
    /// you can return this class from your command and supply a custom audit message. This
    /// audit message will then show up in the Audit Log.
    /// </remarks>
    public class AuditableResult<TResult>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="result">The command response.</param>
        public AuditableResult(TResult result)
            : this(result, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="result">The command response.</param>
        /// <param name="auditMessage">The audit message for the audit log.</param>
        public AuditableResult(TResult result, string auditMessage)
        {
            Result = result;
            AuditMessage = auditMessage;
        }

        /// <summary>
        /// Gets the audit message that will be seen in the audit log.
        /// </summary>
        public string AuditMessage { get; }

        /// <summary>
        /// Gets the command result.
        /// </summary>
        public TResult Result { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(AuditMessage))
            {
                return Result?.ToString();
            }

            return AuditMessage;
        }
    }
}
