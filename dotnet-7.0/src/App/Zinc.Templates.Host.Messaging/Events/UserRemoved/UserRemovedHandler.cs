using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using RedLine.Application.Commands.Grants.RevokeAllGrants;
using RedLine.Domain;

namespace Zinc.Templates.Host.Messaging.Events.UserRemoved
{
    /// <summary>
    /// Handler for the <see cref="Krypton.Authentication.Domain.Events.UserRemoved" /> domain event.
    /// </summary>
    public class UserRemovedHandler : IHandleMessages<Krypton.Authentication.Domain.Events.UserRemoved>
    {
        private readonly IMediator mediator;
        private readonly ITenantId tenantId;
        private readonly ICorrelationId correlationId;
        private readonly ILogger<UserRemovedHandler> logger;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="mediator">The mediator to carry out commands.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation id.</param>
        /// <param name="logger">A logger.</param>
        public UserRemovedHandler(IMediator mediator, ITenantId tenantId, ICorrelationId correlationId, ILogger<UserRemovedHandler> logger)
        {
            this.mediator = mediator;
            this.tenantId = tenantId;
            this.correlationId = correlationId;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task Handle(Krypton.Authentication.Domain.Events.UserRemoved message, IMessageHandlerContext context)
        {
            var command = new RevokeAllGrantsCommand(tenantId.Value, correlationId.Value, message.UserId, message.UserName);

            // this command is being sent as the service account user, not the original user.
            var result = await mediator.Send(command).ConfigureAwait(false);

            if (result.Result)
            {
                logger.LogInformation("Grants have been revoked from {User} on behalf of {RevokingUser}", message.UserName, message.RemovedBy);
            }
            else
            {
                logger.LogWarning(result.AuditMessage);
            }
        }
    }
}
