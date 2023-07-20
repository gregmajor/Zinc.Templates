using System.Threading.Tasks;
using RedLine.Domain.Model;

namespace RedLine.Domain.Repositories
{
    /// <summary>
    /// The interface that defines a contract for querying reference data.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Executes the <see cref="IDataQuery{TConnection,TAggregate}"/>.
        /// </summary>
        /// <param name="qry">The query to execute.</param>
        /// <returns>The results of the query.</returns>
        /// <typeparam name="TConnection">The type of connection needed to run this query.</typeparam>
        /// <typeparam name="TModel">The model returned by the query.</typeparam>
        Task<TModel> Query<TConnection, TModel>(IDataQuery<TConnection, TModel> qry);
    }

    /// <summary>
    /// The interface that defines a contract for repositories that manage <see cref="IAggregateRoot"/>s.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    public interface IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        /// <summary>
        /// Deletes an <see cref="IAggregateRoot"/> from the collection.
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> to delete.</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> Delete(TAggregate aggregate);

        /// <summary>
        /// Gets a value indicating whether an <see cref="IAggregateRoot"/> exists in the collection.
        /// </summary>
        /// <param name="key">The <see cref="IAggregateRoot"/> key value.</param>
        /// <returns>True if the aggregate exists; otherwise, false.</returns>
        Task<bool> Exists(string key);

        /// <summary>
        /// Executes the <see cref="IAggregateQuery{TAggregate}"/>.
        /// </summary>
        /// <param name="qry">The query to execute.</param>
        /// <returns>The results of the query.</returns>
        Task<PageableResult<TAggregate>> Query(IAggregateQuery<TAggregate> qry);

        /// <summary>
        /// Executes the <see cref="IAggregateQuery{TAggregate}"/> for a single result.
        /// </summary>
        /// <param name="qry">The query to execute.</param>
        /// <returns>The results of the query.</returns>
        Task<TAggregate> Read(IAggregateQuery<TAggregate> qry);

        /// <summary>
        /// Reads an <see cref="IAggregateRoot"/> from the collection.
        /// </summary>
        /// <param name="key">The <see cref="IAggregateRoot"/> key value.</param>
        /// <returns>The aggregate root, or null of not found.</returns>
        /// <remarks>
        /// This method returns the ETag for the aggregate in the ETag HTTP header so that
        /// clients can use it during a PUT method to enforce optimistic concurrency.
        /// </remarks>
        Task<TAggregate> Read(string key);

        /// <summary>
        /// Inserts or updates (upserts) an <see cref="IAggregateRoot"/> in the collection.
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> to upsert.</param>
        /// <returns>The number of rows affected.</returns>
        /// <remarks>
        /// This method returns the new ETag for the aggregate in the ETag HTTP header so that
        /// clients can use it during future PUT methods to enforce optimistic concurrency.
        /// </remarks>
        Task<int> Save(TAggregate aggregate);
    }
}
