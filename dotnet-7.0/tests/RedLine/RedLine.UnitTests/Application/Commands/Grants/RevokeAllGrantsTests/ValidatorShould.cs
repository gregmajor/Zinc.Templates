using System;
using System.Linq;
using FluentAssertions;
using RedLine.Application.Commands.Grants.RevokeAllGrants;
using Xunit;

namespace RedLine.UnitTests.Application.Commands.Grants.RevokeAllGrantsTests
{
    public class ValidatorShould
    {
        [Fact]
        public void FailWhenCorrelationIdIsEmpty()
        {
            // Arrange
            var request = new RevokeAllGrantsCommand("tenantId", Guid.Empty, "userId", "UserOne");
            var validator = new RevokeAllGrantsCommandValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Where(x => x.PropertyName.Equals("CorrelationId")).Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenTenantIsNullOrEmpty(string tenantId)
        {
            // Arrange
            var request = new RevokeAllGrantsCommand(tenantId, Guid.NewGuid(), "userId", "UserOne");
            var validator = new RevokeAllGrantsCommandValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Where(x => x.PropertyName.Equals("TenantId")).Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenUserIdIsNullOrEmpty(string userId)
        {
            // Arrange
            var request = new RevokeAllGrantsCommand("tenantId", Guid.NewGuid(), userId, "UserOne");
            var validator = new RevokeAllGrantsCommandValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Where(x => x.PropertyName.Equals("UserId")).Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FailWhenFullNameIsNullOrEmpty(string fullName)
        {
            // Arrange
            var request = new RevokeAllGrantsCommand("tenantId", Guid.NewGuid(), "userId", fullName);
            var validator = new RevokeAllGrantsCommandValidator();

            // Act
            var result = validator.Validate(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Where(x => x.PropertyName.Equals("FullName")).Should().NotBeEmpty();
        }
    }
}
