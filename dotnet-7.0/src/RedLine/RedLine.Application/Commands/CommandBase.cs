using System;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using MediatR;
using RedLine.A3;

namespace RedLine.Application.Commands
{
    /// <summary>
    /// An abstract base command class that provides implementations of various required interfaces.
    /// </summary>
    /// <remarks>Inherit from this base class when the command does NOT return a response.</remarks>
    public abstract class CommandBase : CommandBase<Unit>, IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        protected CommandBase(string tenantId, Guid correlationId)
            : base(tenantId, correlationId)
        {
        }
    }

    /// <summary>
    /// An abstract base command class that provides implementations of various required interfaces.
    /// </summary>
    /// <typeparam name="TResponse">The response type returned by the command.</typeparam>
    /// <remarks>Inherit from this base class when the command returns a response.</remarks>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The two classes are related and small.")]
    public abstract class CommandBase<TResponse> : ICommand<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase{TResponse}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        protected CommandBase(string tenantId, Guid correlationId)
        {
            TenantId = tenantId;
            CorrelationId = correlationId;
        }

        /// <inheritdoc/>
        public IAccessToken AccessToken { get; set; }

        /// <inheritdoc/>
        ActivityType IActivity.ActivityType => ActivityType.Command;

        /// <inheritdoc/>
        public Guid CorrelationId { get; protected init; }

        /// <inheritdoc/>
        public string TenantId { get; protected init; }

        /// <inheritdoc/>
        public TransactionScopeOption TransactionBehavior { get; protected internal init; } = TransactionScopeOption.Required;

        /// <inheritdoc/>
        public IsolationLevel TransactionIsolation { get; protected internal init; } = IsolationLevel.ReadCommitted;

        /// <inheritdoc/>
        public TimeSpan TransactionTimeout { get; protected internal init; } = TimeSpan.FromMinutes(1);

        /// <inheritdoc/>
        public string ActivityName => GetType().Name;

        /// <inheritdoc/>
        public abstract string ActivityDisplayName { get; }

        /// <inheritdoc/>
        public abstract string ActivityDescription { get; }
    }
}
