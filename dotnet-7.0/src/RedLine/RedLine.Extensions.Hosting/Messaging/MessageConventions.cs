using System;
using System.Collections.Concurrent;
using System.Linq;
using NServiceBus;
using RedLine.Domain.Events;

namespace RedLine.Extensions.Hosting.Messaging
{
    /// <summary>
    /// Defines conventions for determining message types.
    /// </summary>
    internal static class MessageConventions
    {
        private static readonly ConcurrentDictionary<Type, bool> MessageIndex = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, bool> CommandIndex = new ConcurrentDictionary<Type, bool>();
        private static readonly ConcurrentDictionary<Type, bool> EventIndex = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// Convention for defining an NServiceBus message.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object that we want to verify is a message.</param>
        /// <returns>Returns a <see cref="bool"/> indicating if the message is a valid message.</returns>
        public static bool IsMessage(this Type type)
        {
            return MessageIndex.GetOrAdd(type, t => t.IsCommand() || t.IsEvent() || (t.IsConcreteClass() && t.IsMessageType<IMessage>(Suffixes.MessageType)));
        }

        /// <summary>
        /// Convention for defining an NServiceBus command.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object that we want to verify is a command.</param>
        /// <returns>Returns a <see cref="bool"/> indicating if the message is a command.</returns>
        public static bool IsCommand(this Type type)
        {
            return CommandIndex.GetOrAdd(type, t => t.IsConcreteClass() && t.IsMessageType<ICommand>(Suffixes.CommandType));
        }

        /// <summary>
        /// Convention for defining an NServiceBus event.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the object that we want to verify is an event.</param>
        /// <returns>Returns a <see cref="bool"/> indicating if the message is an event.</returns>
        public static bool IsEvent(this Type type)
        {
            return EventIndex.GetOrAdd(type, t => t.IsConcreteClass() && (t.IsDomainEvent() || t.IsMessageType<IEvent>(Suffixes.EventType)));
        }

        internal static bool IsDomainEvent(this Type t)
        {
            return t.Implements<IDomainEvent>();
        }

        internal static bool IsMessageType<TInterface>(this Type type, string suffix)
        {
            return type.Implements<TInterface>() || (type.IsInMessagesNamespace() && type.HasSuffix(suffix));
        }

        internal static bool HasSuffix(this Type type, string suffix)
        {
            return type.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsInMessagesNamespace(this Type type)
        {
            return !string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.Split('.').Any(part => part.Equals(Suffixes.MessagesNamespace) || part.Equals(Suffixes.MessagingNamespace));
        }

        internal static bool Implements<TInterface>(this Type type)
        {
            return type.GetInterfaces().Any(i => i == typeof(TInterface));
        }

        internal static bool IsConcreteClass(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsGenericType;
        }

        /// <summary>
        /// String constants for message name conventions.
        /// </summary>
        internal static class Suffixes
        {
            /// <summary>
            /// Namespace suffix for message types.
            /// </summary>
            public static readonly string MessagesNamespace = "Messages";

            /// <summary>
            /// Namespace suffix for message types.
            /// </summary>
            public static readonly string MessagingNamespace = "Messaging";

            /// <summary>
            /// Type name suffix for messages.
            /// </summary>
            public static readonly string MessageType = "Message";

            /// <summary>
            /// Type name suffix for commands.
            /// </summary>
            public static readonly string CommandType = "Command";

            /// <summary>
            /// Type name suffix for events.
            /// </summary>
            public static readonly string EventType = "Event";
        }
    }
}
