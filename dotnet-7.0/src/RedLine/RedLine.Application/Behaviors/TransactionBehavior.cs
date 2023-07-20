using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using MediatR;

namespace RedLine.Application.Behaviors
{
    /// <summary>
    /// An <see cref="IPipelineBehavior{TRequest, TResponse}"/> that wraps the request in a <see cref="TransactionScope"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    internal class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection"/> to the database.</param>
        public TransactionBehavior(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Applies the behavior and executes the next one in the pipeline.
        /// </summary>
        /// <param name="request">The executing request.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the request.</param>
        /// <param name="next">The next behavior in the pipeline.</param>
        /// <returns>The response as a <typeparamref name="TResponse"/>.</returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var activity = request as IAmTransactional;

            if (activity == null)
            {
                return await next().ConfigureAwait(false);
            }

            var transaction = new TransactionScope(
                activity.TransactionBehavior,
                new TransactionOptions
                {
                    Timeout = activity.TransactionTimeout,
                    IsolationLevel = activity.TransactionIsolation,
                },
                TransactionScopeAsyncFlowOption.Enabled);

            using (transaction)
            {
                /* READ
                 * We open the connection here to ensure it enlists in the transaction scope.
                 * If an error occurs, that means the connection is already open and the calling
                 * code probably has a bug or something, or perhaps the code is sending through
                 * the mediator more than once. If the latter is the case, we need to ask why.
                 * If there is a valid reason for it, then perhaps this code needs to be re-worked
                 * a bit to allow for activities to be sent through the mediator more than once,
                 * but that should be a rare exception.
                 * */
                connection.Open();

                var response = await next().ConfigureAwait(false);

                transaction.Complete();

                return response;
            }
        }
    }
}
