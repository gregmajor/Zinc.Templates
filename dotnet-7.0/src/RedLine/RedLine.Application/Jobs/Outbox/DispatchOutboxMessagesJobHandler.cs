using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using RedLine.Data.Outbox;

namespace RedLine.Application.Jobs.Outbox
{
    /// <summary>
    /// A handler for the <see cref="DispatchOutboxMessagesJob"/>.
    /// </summary>
    internal class DispatchOutboxMessagesJobHandler : JobHandlerBase<DispatchOutboxMessagesJob>
    {
        private readonly IOutbox outbox;
        private readonly IMessageSession bus;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="outbox">The <see cref="IOutbox"/>.</param>
        /// <param name="bus">The service bus <see cref="IMessageSession"/>.</param>
        public DispatchOutboxMessagesJobHandler(IOutbox outbox, IMessageSession bus)
        {
            this.outbox = outbox;
            this.bus = bus;
        }

        /// <summary>
        /// A handler for the <see cref="DispatchOutboxMessagesJob"/>.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="cancellationToken">A token used to cancel the request.</param>
        /// <returns>The number of messages dispatched.</returns>
        public override async Task<JobResult> Handle(DispatchOutboxMessagesJob request, CancellationToken cancellationToken)
        {
            var numberOfEventsDispatched = await outbox
                .DispatchEvents(request.DispatcherId, Dispatch)
                .ConfigureAwait(false);

            return numberOfEventsDispatched == 0
                ? JobResult.NoWorkPerformed
                : JobResult.OperationSucceeded;
        }

        /* This method is marked internal to facilitate unit testing */
        internal async Task<int> Dispatch(OutboxRecord outboxRecord)
        {
            var messages = outboxRecord.Messages.ToList();

            if (messages.Count > 0)
            {
                var messageTasks = messages.Select(m => string.IsNullOrEmpty(m.Destination) ? PublishMessage(m) : SendMessage(m));
                await Task.WhenAll(messageTasks).ConfigureAwait(false);
            }

            return messages.Count;
        }

        private async Task SendMessage(OutboxMessage m)
        {
            var options = new SendOptions();
            options.RequireImmediateDispatch();

            if (m.MessageHeaders != null)
            {
                foreach (var pair in m.MessageHeaders)
                {
                    options.SetHeader(pair.Key, pair.Value);
                }
            }

            options.SetMessageId(m.MessageId);
            options.SetDestination(m.Destination);

            await bus.Send(m.MessageBody, options).ConfigureAwait(false);
        }

        private async Task PublishMessage(OutboxMessage m)
        {
            var options = new PublishOptions();
            options.RequireImmediateDispatch();

            if (m.MessageHeaders != null)
            {
                foreach (var pair in m.MessageHeaders)
                {
                    options.SetHeader(pair.Key, pair.Value);
                }
            }

            options.SetMessageId(m.MessageId);

            await bus.Publish(m.MessageBody, options).ConfigureAwait(false);
        }
    }
}
