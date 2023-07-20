using FluentValidation;
using RedLine.Application.Commands;

namespace Zinc.Templates.Application.Commands.DeleteGreeting
{
    /// <summary>
    /// Validator for <see cref="DeleteGreetingCommand"/>.
    /// </summary>
    public class DeleteGreetingCommandValidator : CommandValidator<DeleteGreetingCommand>
    {
        /// <summary>
        /// Initializes an instance of <see cref="DeleteGreetingCommandValidator"/>.
        /// </summary>
        public DeleteGreetingCommandValidator()
        {
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.GreetingId).NotEmpty();
        }
    }
}
