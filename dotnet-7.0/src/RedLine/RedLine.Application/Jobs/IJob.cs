using MediatR;

namespace RedLine.Application.Jobs
{
    /// <summary>
    /// The interface that defines a contract for scheduled jobs, such as Quartz jobs.
    /// </summary>
    public interface IJob : IActivity, IRequest<JobResult>
    {
    }
}
