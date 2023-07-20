using System;
using FluentAssertions;
using Moq;
using RedLine.A3;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.Domain.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Application.A3.AccessTokenTests
{
    [Collection(nameof(UnitTestCollection))]
    public class ConstructorShould
    {
        [Fact]
        public void ThrowDomainExceptionWhenAuthenticatedUserIsDifferent()
        {
            // Arrange
            var authenticationMock = new Mock<IAuthenticationToken>();
            authenticationMock.Setup(x => x.UserId).Returns("user1");
            var authorizationMock = new Mock<IAuthorizationPolicy>();
            authorizationMock.Setup(x => x.UserId).Returns("user2");

            // Act
            Action action = () => new AccessToken(authenticationMock.Object, authorizationMock.Object);

            // Assert
            action.Should().Throw<DomainException>().And.Message.Contains("are for different users.");
        }
    }
}
