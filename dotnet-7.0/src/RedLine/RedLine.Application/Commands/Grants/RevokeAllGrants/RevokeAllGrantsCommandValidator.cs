using FluentValidation;

namespace RedLine.Application.Commands.Grants.RevokeAllGrants
{
    /// <summary>
    /// Validates the <see cref="RevokeAllGrantsCommand"/> command.
    /// </summary>
    public class RevokeAllGrantsCommandValidator : CommandValidator<RevokeAllGrantsCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevokeAllGrantsCommandValidator"/> class.
        /// </summary>
        public RevokeAllGrantsCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.FullName).NotEmpty();
        }
    }
}
