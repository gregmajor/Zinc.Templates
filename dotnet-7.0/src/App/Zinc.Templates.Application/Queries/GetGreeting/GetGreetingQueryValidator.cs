using FluentValidation;
using RedLine.Application.Queries;

namespace Zinc.Templates.Application.Queries.GetGreeting
{
    /// <summary>
    /// Validator for <see cref="GetGreetingQuery"/>.
    /// </summary>
    public class GetGreetingQueryValidator : QueryValidator<GetGreetingQuery>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GetGreetingQueryValidator"/>.
        /// </summary>
        public GetGreetingQueryValidator()
        {
            RuleFor(x => x.GreetingId).NotEmpty();
        }
    }
}
