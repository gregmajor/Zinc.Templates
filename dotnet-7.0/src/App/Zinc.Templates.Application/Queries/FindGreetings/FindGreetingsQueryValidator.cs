using RedLine.Application;
using RedLine.Application.Queries;

namespace Zinc.Templates.Application.Queries.FindGreetings
{
    /// <summary>
    /// Validator for <see cref="FindGreetingsQuery"/>.
    /// </summary>
    public class FindGreetingsQueryValidator : QueryValidator<FindGreetingsQuery>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FindGreetingsQueryValidator"/>.
        /// </summary>
        public FindGreetingsQueryValidator()
        {
            RuleFor(x => x.Pattern).NotNullOrWhitespace();
        }
    }
}
