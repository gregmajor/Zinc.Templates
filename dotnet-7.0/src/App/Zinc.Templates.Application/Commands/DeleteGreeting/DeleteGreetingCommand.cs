using System;
using RedLine.Application.Commands;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Commands.DeleteGreeting
{
    /// <summary>
    /// Delete a greeting by id.
    /// </summary>
    public class DeleteGreetingCommand : ResourceCommandBase<Greeting>
    {
        /// <summary>
        /// Initializes the command.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="greetingId">The greeting identifier.</param>
        public DeleteGreetingCommand(string tenantId, Guid correlationId, Guid greetingId)
            : base(tenantId, correlationId, greetingId.ToString())
        {
            GreetingId = greetingId;
        }

        /// <summary>
        /// The greeting identifier.
        /// </summary>
        public Guid GreetingId { get; }

        /// <inheritdoc/>
        public override string ActivityDescription => "Delete greetings";

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Delete greetings";
    }
}
