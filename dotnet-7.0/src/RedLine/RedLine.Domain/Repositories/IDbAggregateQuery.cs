using RedLine.Domain.Model;

namespace RedLine.Domain.Repositories
{
    /// <summary>
    /// Properties an aggregate query needs when run against a database.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    public interface IDbAggregateQuery<TAggregate> : IAggregateQuery<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        /// <summary>
        /// The command to run against a database, typically Sql.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// The params, if any, for running the command.
        /// </summary>
        object Params { get; }
    }
}
