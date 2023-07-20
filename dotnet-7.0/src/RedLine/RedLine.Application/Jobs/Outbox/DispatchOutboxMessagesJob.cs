using System;
using RedLine.Data.Outbox;

namespace RedLine.Application.Jobs.Outbox
{
    /// <summary>
    /// A job that dispatches messages from the <see cref="IOutbox"/>.
    /// </summary>
    public class DispatchOutboxMessagesJob : JobBase
    {
        internal static readonly string DefaultDispatcherId = Guid.NewGuid().ToString();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="dispatcherId">The id of the dispatcher, in case multiple dispatchers are running in-process.</param>
        public DispatchOutboxMessagesJob(string tenantId, Guid correlationId, string dispatcherId)
            : base(tenantId, correlationId)
        {
            DispatcherId = dispatcherId ?? DefaultDispatcherId;

            /* We set the IsolationLevel to Snapshot here because the outbox dispatchers use a competing
             * consumer pattern, and we need a higher isolation level than normal. The Snapshot isolation,
             * for Postgres at least, is basically read-committed, plus repeatable read, and some rudimentary
             * optimistic concurrency checking. The SQL Server Snapshot isolation is VERY different.
             * https://www.postgresql.org/docs/current/transaction-iso.html
             * */
            TransactionIsolation = System.Transactions.IsolationLevel.Snapshot;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        public DispatchOutboxMessagesJob(string tenantId, Guid correlationId)
            : this(tenantId, correlationId, DefaultDispatcherId)
        { }

        /// <summary>
        /// Gets a unique identifier for the dispatcher.
        /// </summary>
        public string DispatcherId { get; }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Dispatch outbox messages";

        /// <inheritdoc/>
        public override string ActivityDescription => "Starts the job to send all messages from the outbox.";
    }
}
