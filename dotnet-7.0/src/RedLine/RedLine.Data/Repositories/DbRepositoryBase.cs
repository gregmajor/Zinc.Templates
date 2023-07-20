using RedLine.Data.Outbox;
using RedLine.Domain;
using RedLine.Domain.Model;
using RedLine.Domain.Repositories;

namespace RedLine.Data.Repositories
{
    /// <summary>
    /// Provides an implementation of the <see cref="IRepository{TAggregate}"/> interface, specific to databases.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    public abstract class DbRepositoryBase<TAggregate> : RepositoryBase<TAggregate, IDbAggregateQuery<TAggregate>>
        where TAggregate : class, IAggregateRoot
    {
        /// <summary>
        /// Initializes the repository.
        /// </summary>
        /// <param name="context">The activity context.</param>
        /// <param name="outbox">An outbox.</param>
        protected DbRepositoryBase(IActivityContext context, IOutbox outbox)
            : base(context, outbox)
        {
        }
    }
}
