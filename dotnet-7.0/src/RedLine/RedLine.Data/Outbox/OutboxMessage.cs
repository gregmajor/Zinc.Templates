using System;
using System.Collections.Generic;
using RedLine.Domain.Events;

namespace RedLine.Data.Outbox
{
    /// <summary>
    /// An outbox message that is recorded for each <see cref="IDomainEvent"/>.
    /// </summary>
    public class OutboxMessage
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public OutboxMessage()
        {
            MessageId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the body of the message.
        /// </summary>
        public object MessageBody { get; set; }

        /// <summary>
        /// Gets or sets the message headers.
        /// </summary>
        public IReadOnlyDictionary<string, string> MessageHeaders { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the message.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets an optional destination for the message.
        /// </summary>
        public string Destination { get; set; }
    }
}
