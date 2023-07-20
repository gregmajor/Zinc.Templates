using System;
using RedLine.Application.Commands;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Commands.PutGreeting
{
    /// <summary>
    /// Sample update a greeting.
    /// </summary>
    public class PutGreetingCommand : ResourceCommandBase<Greeting, Guid>
    {
        /// <summary>
        /// Make the command.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="greetingId">The greeting identifier.</param>
        /// <param name="message">The greeting message.</param>
        public PutGreetingCommand(string tenantId, Guid correlationId, Guid greetingId, string message)
            : base(tenantId, correlationId, greetingId.ToString())
        {
            GreetingId = greetingId;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the greeting unique identifier.
        /// </summary>
        public Guid GreetingId { get; }

        /// <summary>
        /// Gets or sets the greeting message.
        /// </summary>
        public string Message { get; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Update a greeting's message, or add it if it doesn't exist.";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Update greeting";
    }
}
