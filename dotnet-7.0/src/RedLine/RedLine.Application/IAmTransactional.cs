using System;
using System.Transactions;

namespace RedLine.Application
{
    /// <summary>
    /// The interface that defines a contract for activities that indicate their transactional preferences.
    /// </summary>
    public interface IAmTransactional
    {
        /// <summary>
        /// Gets the transaction behavior as a <see cref="TransactionScopeOption"/>.
        /// </summary>
        TransactionScopeOption TransactionBehavior { get; }

        /// <summary>
        /// Gets the <see cref="TransactionIsolation"/> that should be set for the transaction.
        /// </summary>
        IsolationLevel TransactionIsolation { get; }

        /// <summary>
        /// Gets or set the transaction timeout as a <see cref="TimeSpan"/>.
        /// </summary>
        TimeSpan TransactionTimeout { get; }
    }
}
