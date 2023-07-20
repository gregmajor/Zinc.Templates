using System;
using RedLine.Domain.Events;

namespace Zinc.Templates.Domain.Events
{
    /// <summary>
    /// Domain event raised when a message changes.
    /// </summary>
    public record GreetingMessageChanged : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="greetingId">The greeting unique identifier.</param>
        /// <param name="newMessage">The new message..</param>
        /// <param name="previousMessage">The old greeting message.</param>
        /// <param name="timestamp">The timestamp.</param>
        public GreetingMessageChanged(Guid greetingId, string newMessage, string previousMessage, DateTimeOffset timestamp)
            : this()
        {
            GreetingId = greetingId;
            NewMessage = newMessage;
            PreviousMessage = previousMessage;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Empty constructor for json deserialization.
        /// </summary>
        protected GreetingMessageChanged()
        {
        }

        /// <summary>
        /// Gets the greeting unique identifier.
        /// </summary>
        public Guid GreetingId { get; }

        /// <summary>
        /// Gets the new greeting message.
        /// </summary>
        public string NewMessage { get; }

        /// <summary>
        /// Gets the previous greeting message.
        /// </summary>
        public string PreviousMessage { get; }

        /// <summary>
        /// Gets the greeting timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; }
    }
}
