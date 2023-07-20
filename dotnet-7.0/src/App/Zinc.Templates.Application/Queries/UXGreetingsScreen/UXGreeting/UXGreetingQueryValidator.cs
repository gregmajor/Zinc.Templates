using FluentValidation;
using RedLine.Application.Queries;

namespace Zinc.Templates.Application.Queries.UXGreetingsScreen.UXGreeting
{
    /// <summary>
    /// Validator for <see cref="UXGreetingQuery"/>.
    /// </summary>
    public class UXGreetingQueryValidator : QueryValidator<UXGreetingQuery>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UXGreetingQueryValidator"/>.
        /// </summary>
        public UXGreetingQueryValidator()
        {
            RuleFor(x => x.GreetingId).NotEmpty();
        }
    }
}
