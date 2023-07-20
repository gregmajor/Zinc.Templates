using FluentValidation;

namespace RedLine.Application.ActivityValidation
{
    /// <summary>
    /// Validator for <see cref="IAmAuthorizableForResource"/> command, query, job, or notification.
    /// </summary>
    /// <typeparam name="TRequest">The command, query, job, or notification to validate.</typeparam>
    public class AuthorizableForResourceValidator<TRequest> : RulesFor<TRequest, IAmAuthorizableForResource>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AuthorizableForResourceValidator{TRequest}"/>.
        /// </summary>
        public AuthorizableForResourceValidator()
        {
            RuleFor(x => x.ResourceId).NotNullOrWhitespace();
            RuleFor(x => x.ResourceTypes)
                .NotNull()
                .NotEmpty();
            RuleForEach(x => x.ResourceTypes)
                .NotNullOrWhitespace();
        }
    }
}
