using System.Data;
using RedLine.Domain.Repositories;

namespace RedLine.Data.Repositories
{
    /// <summary>
    /// An abstract base class providing an implementation of <see cref="IDataQuery{IDbConnection, TModel}"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of model returned from the query.</typeparam>
    public abstract class DbDataQuery<TModel> : DataQueryBase<IDbConnection, TModel>
    {
    }
}
