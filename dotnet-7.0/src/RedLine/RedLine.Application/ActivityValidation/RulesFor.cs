using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace RedLine.Application.ActivityValidation
{
    /// <summary>
    /// Validator for the <typeparamref name="TPart"/> properties of <typeparamref name="TRequest"/>.
    /// </summary>
    /// <typeparam name="TRequest">The command, query, job, or notification.</typeparam>
    /// <typeparam name="TPart">An interface implemented by <typeparamref name="TRequest"/>.</typeparam>
    public abstract class RulesFor<TRequest, TPart> : AbstractValidator<TPart>, IValidator<TRequest>
    {
        /// <inheritdoc cref="IValidator{T}.Validate(T)"/>
        public ValidationResult Validate(TRequest instance)
        {
            return Validate((TPart)(object)instance);
        }

        /// <inheritdoc cref="IValidator{T}.ValidateAsync(T, CancellationToken)"/>
        public Task<ValidationResult> ValidateAsync(TRequest instance, CancellationToken cancellation = default)
        {
            return ValidateAsync((TPart)(object)instance, cancellation);
        }
    }
}
