using System;
using System.Linq;
using FluentValidation;

namespace RedLine.Application
{
    /// <summary>
    /// Additional validations for Fluent Validator.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Is a valid tenant id.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, string> IsValidTenantId<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotNullOrWhitespace();
        }

        /// <summary>
        /// Is a valid correlation id.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, Guid> IsValidCorrelationId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} must not be an empty guid.");
        }

        /// <summary>
        /// Is valid page number for a pageable query.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, int> IsValidPageNumber<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(1)
                .WithMessage("{PropertyName} must not be greater than or equal to {ComparisonValue}.");
        }

        /// <summary>
        /// Is valid page size for a pageable query.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <param name="maxPageSize">The maximum page size for a pageable query.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, int> IsValidPageSize<T>(this IRuleBuilder<T, int> ruleBuilder, int maxPageSize)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(1)
                .WithMessage("{PropertyName} must not be greater than or equal to {ComparisonValue}.")
                .LessThanOrEqualTo(maxPageSize)
                .WithMessage("{PropertyName} must not be greater than or equal to {ComparisonValue}.");
        }

        /// <summary>
        /// The string is not all whitespace.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, string> NotWhitespace<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(s => s == null || !s.All(char.IsWhiteSpace))
                .WithMessage("{PropertyName} must not be all whitespace.");
        }

        /// <summary>
        /// The string must not be null or empty. All whitespace is okay.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, string> NotNullOrEmpty<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage("{PropertyName} must not be null.")
                .NotEmpty().WithMessage("{PropertyName} must not be an empty string.");
        }

        /// <summary>
        /// The string must not be null, empty, or all whitespace.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, string> NotNullOrWhitespace<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotNullOrEmpty()
                .NotWhitespace();
        }

        /// <summary>
        /// The string must not be a guid.
        /// </summary>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <typeparam name="T"><inheritdoc cref="IRuleBuilder{T,TProperty}"/></typeparam>
        /// <returns>The <paramref name="ruleBuilder"/>.</returns>
        public static IRuleBuilderOptions<T, string> NotGuidString<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(s => !Guid.TryParse(s, out _))
                .WithMessage("{PropertyName} must not be a guid. Provided value is {PropertyValue}");
        }
    }
}
