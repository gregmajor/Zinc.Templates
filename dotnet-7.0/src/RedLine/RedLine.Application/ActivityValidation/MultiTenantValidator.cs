using FluentValidation;

namespace RedLine.Application.ActivityValidation
{
    /// <summary>
    /// Validates a <see cref="IAmMultiTenant"/> command or query.
    /// </summary>
    /// <typeparam name="TRequest">The command, query, job or notification to validate.</typeparam>
    public class MultiTenantValidator<TRequest> : RulesFor<TRequest, IAmMultiTenant>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultiTenantValidator{TRequest}"/>.
        /// </summary>
        public MultiTenantValidator()
        {
            RuleFor(x => x.TenantId).IsValidTenantId();
        }
    }
}
