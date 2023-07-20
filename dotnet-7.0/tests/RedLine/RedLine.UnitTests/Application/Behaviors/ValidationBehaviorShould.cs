using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using RedLine.Application.Behaviors;
using RedLine.Application.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Application.Behaviors
{
    [Collection(nameof(UnitTestCollection))]
    public class ValidationBehaviorShould
    {
        private readonly Mock<IValidator<object>> passingValidatorMock = new();
        private readonly Mock<IValidator<object>> failingValidatorMock = new();
        private readonly ValidationFailure failure = new("property-name", "error-message");
        private readonly object request = new();
        private readonly object response = new();

        public ValidationBehaviorShould()
        {
            passingValidatorMock.Setup(x => x.Validate(request)).Returns(new ValidationResult());
            failingValidatorMock.Setup(x => x.Validate(request)).Returns(new ValidationResult(new[] { failure }));
        }

        [Fact]
        public async Task ThrowInvalidCommandOrQueryExceptionIfAValidatonFails()
        {
            // Arrange
            var behavior = new ValidationBehavior<object, object>(new[] { failingValidatorMock.Object });

            // Act
            Func<Task<object>> action = () => behavior.Handle(request, CancellationToken.None, Next);

            // Assert
            await action.Should().ThrowAsync<InvalidCommandOrQueryException>().ConfigureAwait(false);
        }

        private Task<object> Next() => Task.FromResult(response);
    }
}
