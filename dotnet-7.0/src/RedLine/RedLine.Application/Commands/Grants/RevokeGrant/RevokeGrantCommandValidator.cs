using FluentValidation;

namespace RedLine.Application.Commands.Grants.RevokeGrant
{
    /// <summary>
    /// Validates the <see cref="RevokeGrantCommand"/>.
    /// </summary>
    public class RevokeGrantCommandValidator : CommandValidator<RevokeGrantCommand>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RevokeGrantCommandValidator()
        {
            RuleFor(x => x.GrantType).NotEmpty();
            RuleFor(x => x.Qualifier).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
