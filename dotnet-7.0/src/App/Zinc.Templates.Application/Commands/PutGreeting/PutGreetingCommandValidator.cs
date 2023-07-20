using FluentValidation;
using RedLine.Application.Commands;

namespace Zinc.Templates.Application.Commands.PutGreeting
{
    /// <summary>
    /// Validator for <see cref="PutGreetingCommand"/>.
    /// </summary>
    public class PutGreetingCommandValidator : CommandValidator<PutGreetingCommand>
    {
        /// <summary>
        /// Initializes an instance of <see cref="PutGreetingCommandValidator"/>.
        /// </summary>
        public PutGreetingCommandValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.GreetingId).NotEmpty();
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
