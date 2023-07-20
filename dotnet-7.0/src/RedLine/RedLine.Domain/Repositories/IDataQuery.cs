using System;
using System.Threading.Tasks;

namespace RedLine.Domain.Repositories
{
    /// <summary>
    /// The interface that defines a contract for a query that can be executed by a <see cref="IRepository" />.
    /// The interface supports paging, but note that paging is optional and not required. Also,
    /// a given repository may or may not support paging.
    /// </summary>
    /// <typeparam name="TConnection">The type of connection needed to run this query.</typeparam>
    /// <typeparam name="TModel">The model returned by the query.</typeparam>
    public interface IDataQuery<TConnection, TModel>
    {
        /// <summary>
        /// Gets a function that can be used to execute the query.
        /// </summary>
        /// <remarks>This property is set by the implementing class.</remarks>
        Func<TConnection, Task<TModel>> Resolve { get; }
    }
}
