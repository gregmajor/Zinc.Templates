using System;
using System.Collections.Generic;
using RedLine.Domain.Events;

namespace RedLine.Domain.Model
{
    /// <summary>
    /// An abstract base class for aggregate roots.
    /// </summary>
    public abstract class AggregateRootBase : EntityBase, IAggregateRoot
    {
        private readonly List<IDomainEvent> domainEvents = new();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected AggregateRootBase()
        { }

        /// <inheritdoc/>
        public string ETag { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        IEnumerable<IDomainEvent> IAggregateRoot.Events => domainEvents;

        /// <summary>
        /// Adds the domain event to the internal collection.
        /// </summary>
        /// <typeparam name="TEvent">The type of domain event to raise.</typeparam>
        /// <param name="evt">The instance of the domain event.</param>
        protected void RaiseEvent<TEvent>(TEvent evt)
            where TEvent : IDomainEvent
        {
            domainEvents.Add(evt);
        }
    }
}
