namespace RedLine.Application.ActivityValidation
{
    /// <summary>
    /// Validates a <see cref="IAmCorrelatable"/> command or query.
    /// </summary>
    /// <typeparam name="TRequest">The command, query, job or notification to validate.</typeparam>
    public class CorrelatableValidator<TRequest> : RulesFor<TRequest, IAmCorrelatable>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IAmCorrelatable"/>.
        /// </summary>
        public CorrelatableValidator()
        {
            RuleFor(x => x.CorrelationId).IsValidCorrelationId();
        }
    }
}
