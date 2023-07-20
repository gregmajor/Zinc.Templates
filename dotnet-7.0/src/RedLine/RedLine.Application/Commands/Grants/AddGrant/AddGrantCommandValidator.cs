using System;
using FluentValidation;

namespace RedLine.Application.Commands.Grants.AddGrant
{
    /// <summary>
    /// Validates the <see cref="AddGrantCommand"/>.
    /// </summary>
    public class AddGrantCommandValidator : CommandValidator<AddGrantCommand>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AddGrantCommandValidator()
        {
            RuleFor(x => x.ExpiresOn)
                .Must(x => x == null || (x.GetValueOrDefault().ToUniversalTime() >= DateTimeOffset.UtcNow))
                .WithMessage("{PropertyName} must be null or in the future.");
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.GrantType).NotEmpty();
            RuleFor(x => x.Qualifier).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
