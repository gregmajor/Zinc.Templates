using RedLine.Application.ActivityValidation;

namespace RedLine.Application.Jobs
{
    /// <summary>
    /// Validates a <typeparamref name="TJob"/>.
    /// </summary>
    /// <typeparam name="TJob">The type of command to validate.</typeparam>
    public abstract class JobValidator<TJob> : RequestValidator<TJob>
        where TJob : IJob
    {
    }
}
