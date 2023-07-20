using System;
using RedLine.Domain.Model;
using Zinc.Templates.Domain.Events;

namespace Zinc.Templates.Domain.Model
{
    /// <summary>
    /// Represents a greeting, such as Hello!.
    /// </summary>
    public class Greeting : AggregateRootBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="greetingId">The aggregate unique identifier.</param>
        /// <param name="message">The greeting message.</param>
        /// <param name="timestamp">The timestamp.</param>
        public Greeting(Guid greetingId, string message, DateTimeOffset timestamp)
        {
            GreetingId = greetingId;
            Message = message;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected Greeting()
        {
        }

        /// <summary>
        /// Gets the unique identifier for the aggregate.
        /// </summary>
        public Guid GreetingId { get; protected set; }

        /// <inheritdoc />
        public override string Key => GreetingId.ToString();

        /// <summary>
        /// Gets the greeting message.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Gets the greeting timestamp.
        /// </summary>
        public DateTimeOffset Timestamp { get; protected set; }

        /// <summary>
        /// Update the message.
        /// </summary>
        /// <param name="newMessage">The new message.</param>
        public void UpdateMessage(string newMessage)
        {
            var oldMessage = Message;
            Message = newMessage;
            Timestamp = DateTimeOffset.UtcNow;
            RaiseEvent(new GreetingMessageChanged(GreetingId, Message, oldMessage, Timestamp));
        }
    }
}
