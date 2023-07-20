using FluentValidation;

namespace RedLine.Application.ActivityValidation
{
    /// <summary>
    /// Validates a command, query, job, or notification.
    /// </summary>
    /// <typeparam name="TRequest">The thing being validated.</typeparam>
    public abstract class RequestValidator<TRequest> : AbstractValidator<TRequest>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RequestValidator{TRequest}"/>.
        /// </summary>
        protected RequestValidator()
        {
            if (typeof(TRequest).IsAssignableTo(typeof(IAmMultiTenant)))
            {
                Include(new MultiTenantValidator<TRequest>());
            }

            if (typeof(TRequest).IsAssignableTo(typeof(IAmCorrelatable)))
            {
                Include(new CorrelatableValidator<TRequest>());
            }

            if (typeof(TRequest).IsAssignableTo(typeof(IActivity)))
            {
                Include(new ActivityValidator<TRequest>());
            }

            if (typeof(TRequest).IsAssignableTo(typeof(IAmAuthorizableForResource)))
            {
                Include(new AuthorizableForResourceValidator<TRequest>());
            }
        }
    }
}
