using System;
using System.Transactions;
using RedLine.A3;

namespace RedLine.Application.Queries
{
    /// <summary>
    /// An abstract base query class that provides implementations of various required interfaces.
    /// </summary>
    /// <typeparam name="TResponse">The response type returned by the query.</typeparam>
    public abstract class QueryBase<TResponse> : IQuery<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase{TResponse}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        protected QueryBase(string tenantId, Guid correlationId)
        {
            TenantId = tenantId;
            CorrelationId = correlationId;
        }

        /// <inheritdoc/>
        public IAccessToken AccessToken { get; set; }

        /// <inheritdoc/>
        ActivityType IActivity.ActivityType => ActivityType.Query;

        /// <inheritdoc/>
        public Guid CorrelationId { get; protected init; }

        /// <inheritdoc/>
        public string TenantId { get; protected init; }

        /// <inheritdoc/>
        public TransactionScopeOption TransactionBehavior { get; protected internal init; } = TransactionScopeOption.Required;

        /// <inheritdoc/>
        public IsolationLevel TransactionIsolation { get; protected internal init; } = IsolationLevel.ReadCommitted;

        /// <inheritdoc/>
        public TimeSpan TransactionTimeout { get; protected internal init; } = TimeSpan.FromMinutes(2);

        /// <inheritdoc/>
        public string ActivityName => GetType().Name;

        /// <inheritdoc/>
        public abstract string ActivityDisplayName { get; }

        /// <inheritdoc/>
        public abstract string ActivityDescription { get; }
    }
}
