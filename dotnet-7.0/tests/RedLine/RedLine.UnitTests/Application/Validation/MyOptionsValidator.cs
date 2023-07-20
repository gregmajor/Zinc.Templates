using FluentValidation;

namespace RedLine.UnitTests.Application.Validation
{
    public class MyOptionsValidator : AbstractValidator<MyOptions>
    {
        public MyOptionsValidator()
        {
            RuleFor(x => x.FailTheTest).Must(x => x.Equals(false));
        }
    }
}
