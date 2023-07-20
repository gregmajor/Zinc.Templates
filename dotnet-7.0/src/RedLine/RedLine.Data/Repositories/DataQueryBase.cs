using System;
using System.Threading.Tasks;
using RedLine.Domain.Repositories;

namespace RedLine.Data.Repositories
{
    /// <summary>
    /// An abstract base class providing an implementation of <see cref="IDataQuery{TConnection, TModel}"/>.
    /// </summary>
    /// <typeparam name="TConnection">The type of connection needed to run the query.</typeparam>
    /// <typeparam name="TModel">The type of model returned from the query.</typeparam>
    public abstract class DataQueryBase<TConnection, TModel> : IDataQuery<TConnection, TModel>
    {
        /// <inheritdoc/>
        public Func<TConnection, Task<TModel>> Resolve { get; protected init; }
    }
}
