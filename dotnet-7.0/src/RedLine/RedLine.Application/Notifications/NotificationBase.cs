using System;
using System.Transactions;
using RedLine.A3;

namespace RedLine.Application.Notifications
{
    /// <summary>
    /// An abstract base class providing an implementation of the <see cref="INotification"/> interface.
    /// </summary>
    public abstract class NotificationBase : INotification
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        protected NotificationBase(string tenantId, Guid correlationId)
        {
            TenantId = tenantId;
            CorrelationId = correlationId;
        }

        /// <inheritdoc/>
        public IAccessToken AccessToken { get; set; }

        /// <inheritdoc/>
        ActivityType IActivity.ActivityType => ActivityType.Notification;

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
