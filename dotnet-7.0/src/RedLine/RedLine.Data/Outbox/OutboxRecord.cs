using System;
using System.Collections.Generic;
using RedLine.Domain.Events;

namespace RedLine.Data.Outbox
{
    /// <summary>
    /// Represents an outbox record, which contains all the domain events published in a given transaction.
    /// </summary>
    public class OutboxRecord
    {
        /// <summary>
        /// Gets or sets the dispatcher that is responsible for sending the message.
        /// </summary>
        public string DispatcherId { get; set; }

        /// <summary>
        /// Gets or sets the dispatcher timeout in UTC. After this timeout, the message is available for another dispatcher to publish.
        /// </summary>
        public DateTime? DispatcherTimeout { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the outbox record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainEvent"/> messages that were published in the transaction.
        /// </summary>
        public IEnumerable<OutboxMessage> Messages { get; set; }
    }
}
