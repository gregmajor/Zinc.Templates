using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RedLine.Application.Jobs
{
    /// <summary>
    /// An abstract base class that provides an implementation of <see cref="IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The job being handled.</typeparam>
    public abstract class JobHandlerBase<TRequest> : IRequestHandler<TRequest, JobResult>
        where TRequest : IJob
    {
        /// <summary>
        /// Handles <typeparamref name="TRequest"/>.
        /// </summary>
        /// <param name="request">The job to be executed.</param>
        /// <param name="cancellationToken">A cancellation token to abort the task.</param>
        /// <returns>Either <see cref="JobResult"/>.NoWorkPerformed or OperationSucceeded.</returns>
        public abstract Task<JobResult> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
