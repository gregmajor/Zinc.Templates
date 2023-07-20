using RedLine.Application.ActivityValidation;

namespace RedLine.Application.Queries
{
    /// <summary>
    /// Validates a <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of command to validate.</typeparam>
    public abstract class QueryValidator<TQuery> : RequestValidator<TQuery>
        where TQuery : IQuery
    {
    }
}
