using RedLine.Domain.Model;
using RedLine.Domain.Repositories;

namespace RedLine.Data.Repositories
{
    /// <summary>
    /// Query that goes against a database.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    public abstract class DbAggregateQueryBase<TAggregate> : IDbAggregateQuery<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        /// <inheritdoc />
        public string Command { get; protected init; }

        /// <inheritdoc />
        public object Params { get; protected init; }
    }
}
