using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace RedLine.Application
{
    // Options validation built-into FluentValidation is tbd
    // see: https://github.com/JeremySkinner/FluentValidation/issues/969.
    // In the meantime, let's make a custom forwarder.

    /// <summary>
    /// Validates the application's options.
    /// </summary>
    public static class OptionsBuilderExtensions
    {
        /// <summary>Build a function that will use FluentValidation to validate options.</summary>
        /// <typeparam name="TOptions">Options to validate.</typeparam>
        /// <typeparam name="TValidator">Validator to use for validation.</typeparam>
        /// <param name="builder">OptionsBuilder being configured for validation.</param>
        public static void Validate<TOptions, TValidator>(this OptionsBuilder<TOptions> builder)
            where TOptions : class, new()
            where TValidator : IValidator<TOptions>, new()
        {
            bool ValidateOptionsInstance(TOptions optionsInstance)
            {
                var validator = new TValidator();
                var rulesetDiscriminator = validator as IDetermineValidationRuleSetByConfig<TOptions>;
                ValidationResult result = validator.Validate(optionsInstance, options =>
                {
                    options.IncludeRuleSets(rulesetDiscriminator?.WhichRuleSets(optionsInstance));
                });

                if (result.IsValid)
                {
                    return true;
                }

                // by default, the built-in IOptions validation strings multiple `.Validate` calls -
                //    one for each property. Then it throws an OptionValidationFailure after all validations
                //    have been run. We're going to short circuit validation since we validated the entire object,
                //    and we're going to throw an exception
                IEnumerable<string> errors = result
                    .Errors
                    .Select(x => Environment.NewLine + " -- " + x.PropertyName + ": " + x.ErrorMessage);
                string message = $"Validation failed for {typeof(TOptions).FullName}: {string.Join(string.Empty, errors)}";
                throw new ValidationException(message, result.Errors);
            }

            builder.Validate(ValidateOptionsInstance);
        }
    }
}
