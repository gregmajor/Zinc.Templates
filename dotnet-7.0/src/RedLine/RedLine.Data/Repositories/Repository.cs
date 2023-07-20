using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Data.Exceptions;
using RedLine.Domain;
using RedLine.Domain.Exceptions;
using RedLine.Domain.Repositories;

namespace RedLine.Data.Repositories
{
    /// <summary>
    /// Provides an implementation of the <see cref="IRepository"/> interface.
    /// </summary>
    public class Repository : IRepository
    {
        private readonly IActivityContext context;

        /// <summary>
        /// Initializes the repository.
        /// </summary>
        /// <param name="context">The activity context.</param>
        public Repository(IActivityContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public async Task<TModel> Query<TConnection, TModel>(IDataQuery<TConnection, TModel> qry)
        {
            try
            {
                var connection = context.ServiceProvider().GetRequiredService<TConnection>();
                return await qry.Resolve(connection).ConfigureAwait(false);
            }
            catch (Exception e) when (e is not RedLineException)
            {
                throw new OperationFailedException(
                    TypeNameHelper.GetTypeDisplayName(typeof(TModel), false, true),
                    TypeNameHelper.GetTypeDisplayName(qry.GetType(), false, true),
                    e);
            }
        }
    }
}
