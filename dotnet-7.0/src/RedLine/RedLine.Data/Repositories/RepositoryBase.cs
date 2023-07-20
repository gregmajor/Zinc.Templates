using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RedLine.Data.Exceptions;
using RedLine.Data.Outbox;
using RedLine.Domain;
using RedLine.Domain.Exceptions;
using RedLine.Domain.Model;
using RedLine.Domain.Repositories;

namespace RedLine.Data.Repositories
{
    /* READ
     * This base class doesn't do a whole lot. It provides some standard exception handling
     * and deals with ETags and the Outbox. Implementations still have to implement methods
     * to make the actual calls to the database.
     *
     * There may be some use cases that call for more fexlibility in how the repository works,
     * so by all means don't feel obligated to use this class, but if it saves you a little
     * time, then use it!
     * */

    /// <summary>
    /// Provides an implementation of the <see cref="IRepository{TAggregate}"/> interface.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    /// <typeparam name="TAggregateQuery">The type of supported aggregate queries.</typeparam>
    public abstract class RepositoryBase<TAggregate, TAggregateQuery> : IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
        where TAggregateQuery : class, IAggregateQuery<TAggregate>
    {
        /// <summary>
        /// The <see cref="IOutbox"/> for the current request.
        /// </summary>
        private readonly IOutbox outbox;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/> for the current request.</param>
        /// <param name="outbox">The <see cref="IOutbox"/> used to save domain events.</param>
        protected RepositoryBase(IActivityContext context, IOutbox outbox)
        {
            Context = context;
            this.outbox = outbox;
        }

        /// <summary>
        /// Gets the <see cref="IActivityContext"/> for the current request.
        /// </summary>
        protected IActivityContext Context { get; }

        /// <inheritdoc/>
        public async Task<int> Delete(TAggregate aggregate)
        {
            try
            {
                var etag = Context.ETag();

                var rowsAffected = await DeleteInternal(aggregate, etag)
                    .ConfigureAwait(false);

                if (rowsAffected == 0 && !string.IsNullOrEmpty(etag))
                {
                    throw new ConcurrencyException(
                        TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                        aggregate.Key);
                }

                await outbox.SaveEvents(aggregate).ConfigureAwait(false);

                Context.ETag(null);

                return rowsAffected;
            }
            catch (Exception e) when (e is not RedLineException)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                    nameof(Delete),
                    e);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> Exists(string key)
        {
            try
            {
                return await ExistsInternal(key).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not RedLineException)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                    nameof(Exists),
                    e);
            }
        }

        /// <inheritdoc/>
        public async Task<PageableResult<TAggregate>> Query(IAggregateQuery<TAggregate> qry)
        {
            try
            {
                var result = await QueryInternal(EnforceQueryType(qry))
                    .ConfigureAwait(false);

                var etags = result.Items
                    .Where(x => !string.IsNullOrEmpty(x.ETag))
                    .Select(x => $"{x.Key}:{x.ETag}");

                Context.ETag(string.Join(',', etags));

                return result;
            }
            catch (Exception e) when (e is not RedLineException)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                    nameof(Query),
                    e);
            }
        }

        /// <inheritdoc/>
        public Task<TAggregate> Read(string key)
            => Read(() => ReadInternal(key));

        /// <inheritdoc />
        public Task<TAggregate> Read(IAggregateQuery<TAggregate> qry)
            => Read(() => ReadInternal(EnforceQueryType(qry)));

        /// <inheritdoc/>
        public async Task<int> Save(TAggregate aggregate)
        {
            try
            {
                var etag = Context.ETag();

                var newETag = Guid.NewGuid().ToString();

                var rowsAffected = await SaveInternal(aggregate, etag, newETag)
                    .ConfigureAwait(false);

                if (rowsAffected == 0)
                {
                    throw new ConcurrencyException(
                        TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                        aggregate.Key);
                }

                await outbox.SaveEvents(aggregate).ConfigureAwait(false);

                Context.ETag(newETag);

                aggregate.ETag = newETag;

                return rowsAffected;
            }
            catch (Exception e) when (e is not RedLineException)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                    nameof(Save),
                    e);
            }
        }

        /// <summary>
        /// Executes the <see cref="IAggregateQuery{TAggregate}"/> for a single result.
        /// </summary>
        /// <param name="qry">The query to execute.</param>
        /// <returns>The aggregate root, or null if not found.</returns>
        protected virtual Task<TAggregate> ReadInternal(TAggregateQuery qry) => throw new NotImplementedException();

        /// <summary>
        /// Reads an <see cref="IAggregateRoot"/> from the collection.
        /// </summary>
        /// <param name="key">The <see cref="IAggregateRoot"/> key value.</param>
        /// <returns>The aggregate root, or null if not found.</returns>
        protected virtual Task<TAggregate> ReadInternal(string key) => throw new System.NotImplementedException();

        /// <summary>
        /// Deletes an <see cref="IAggregateRoot"/> from the collection.
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> to delete.</param>
        /// <param name="etag">The ETag provided by the client, or null if not provided.</param>
        /// <returns>The number of rows affected.</returns>
        protected virtual Task<int> DeleteInternal(TAggregate aggregate, string etag) => throw new System.NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether an <see cref="IAggregateRoot"/> exists in the collection.
        /// </summary>
        /// <param name="key">The <see cref="IAggregateRoot"/> key value.</param>
        /// <returns>True if the aggregate exists; otherwise, false.</returns>
        protected virtual Task<bool> ExistsInternal(string key) => throw new System.NotImplementedException();

        /// <summary>
        /// Inserts or updates (upserts) an <see cref="IAggregateRoot"/> in the collection.
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> to upsert.</param>
        /// <param name="etag">The ETag provided by the client, or null if not provided.</param>
        /// <param name="newETag">The new ETag that should replace the current one on a successful save.</param>
        /// <returns>The number of rows affected.</returns>
        protected virtual Task<int> SaveInternal(TAggregate aggregate, string etag, string newETag) => throw new System.NotImplementedException();

        /// <summary>
        /// Executes the aggregate query.
        /// </summary>
        /// <param name="qry">The query.</param>
        /// <returns>A <see cref="PageableResult{TAggregate}"/>.</returns>
        protected virtual Task<PageableResult<TAggregate>> QueryInternal(TAggregateQuery qry)
            => throw new NotImplementedException();

        private static TAggregateQuery EnforceQueryType(IAggregateQuery<TAggregate> qry)
        {
            if (qry is not TAggregateQuery query)
            {
                var aggregateName = TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false);
                var queryType = TypeNameHelper.GetTypeDisplayName(qry.GetType(), false, true);
                throw new InvalidOperationException($"The repository for '{aggregateName}' does not support '{queryType}'.");
            }

            return query;
        }

        private async Task<TAggregate> Read(Func<Task<TAggregate>> readOperation)
        {
            try
            {
                var result = await readOperation().ConfigureAwait(false);

                Context.ETag(result?.ETag);

                return result;
            }
            catch (Exception e) when (e is not RedLineException)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(TAggregate), false, false),
                    nameof(Read),
                    e);
            }
        }
    }
}
