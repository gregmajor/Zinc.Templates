using FluentValidation;

namespace RedLine.Application.ActivityValidation
{
    /// <summary>
    /// Validator for <see cref="IActivity"/> command, query, job or notification.
    /// </summary>
    /// <typeparam name="TRequest">The command, query, job or notification to validate.</typeparam>
    public class ActivityValidator<TRequest> : RulesFor<TRequest, IActivity>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ActivityValidator{TCommand}"/>.
        /// </summary>
        public ActivityValidator()
        {
            RuleFor(x => x.ActivityName).NotNullOrWhitespace();
            RuleFor(x => x.ActivityDisplayName).NotNullOrWhitespace();
            RuleFor(x => x.ActivityDescription).NotNullOrWhitespace();
            RuleFor(x => x.ActivityType)
                .NotEqual(ActivityType.Unknown)
                .WithMessage("{PropertyName} must not be {PropertyValue}. It should probably be Command or Query.");
        }
    }
}
