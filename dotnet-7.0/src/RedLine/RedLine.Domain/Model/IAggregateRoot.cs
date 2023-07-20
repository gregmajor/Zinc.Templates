using System.Collections.Generic;
using RedLine.Domain.Events;

namespace RedLine.Domain.Model
{
    /// <summary>
    /// A marker interface for aggregate roots.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Gets or sets the ETag, or concurrency identifier, for the aggregate.
        /// </summary>
        /// <remarks>
        /// <para>
        /// RedLine uses optimistic concurrency based on the ETag concept from the HTTP spec.
        /// During a PUT operation, clients should submit the If-Match header containing the
        /// ETag of the original aggregate they received. If the provided ETag matches what
        /// is in the database, the aggregate will be saved. If not, a 412 error code will be
        /// returned to the client.
        /// </para>
        /// <para>
        /// Note that support for an ETag is up to the aggregate designer. Nothing actually
        /// requires the ETag be supported, but it is certainly encouraged. The actual value
        /// of the ETag should be the string version of a GUID to ensure global uniqueness.
        /// </para>
        /// </remarks>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the domain events raised by the aggregate during the current request.
        /// </summary>
        public IEnumerable<IDomainEvent> Events { get; }
    }
}
