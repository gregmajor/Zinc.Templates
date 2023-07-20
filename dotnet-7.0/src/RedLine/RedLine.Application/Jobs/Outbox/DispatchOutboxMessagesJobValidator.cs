using System.Transactions;
using FluentValidation;

namespace RedLine.Application.Jobs.Outbox
{
    /// <summary>
    /// Validates the <see cref="DispatchOutboxMessagesJob"/>.
    /// </summary>
    public class DispatchOutboxMessagesJobValidator : JobValidator<DispatchOutboxMessagesJob>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DispatchOutboxMessagesJobValidator()
        {
            RuleFor(x => x.DispatcherId).NotEmpty();
            RuleFor(x => x.TransactionIsolation)
                .Must(isolation => isolation == IsolationLevel.Snapshot || isolation == IsolationLevel.Serializable)
                .WithMessage("{PropertyName} must be Snapshot (for Postgres only) or Serializable (for either Postgres or SQL Server).");
        }
    }
}
