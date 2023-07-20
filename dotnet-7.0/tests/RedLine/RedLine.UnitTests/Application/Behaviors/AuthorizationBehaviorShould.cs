using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RedLine.A3;
using RedLine.A3.Authentication;
using RedLine.Application;
using RedLine.Application.Behaviors;
using RedLine.Application.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Application.Behaviors
{
    [Collection(nameof(UnitTestCollection))]
    public class AuthorizationBehaviorShould
    {
        private readonly Mock<IAccessToken> accessTokenMock = new();
        private readonly AuthorizationBehavior<object, object> behavior;
        private readonly Mock<IAmAuthorizable> requestMock = new();
        private readonly Mock<IAmAuthorizableForResource> resourceRequestMock = new();
        private readonly object expectedReturn = new();
        private bool nextCalled = false;

        public AuthorizationBehaviorShould()
        {
            behavior = new(accessTokenMock.Object);
        }

        [Fact]
        public async Task CallNextIfRequestDoesNotImplementIAmAuthorizable()
        {
            // Arrange
            // Act
            var result = await behavior.Handle(new object(), CancellationToken.None, Next).ConfigureAwait(false);

            // Assert
            nextCalled.Should().BeTrue();
            result.Should().Be(expectedReturn);
        }

        [Fact]
        public async Task ThrowNotAuthorizedExceptionIfRequestIsAnonymous()
        {
            // Arrange
            accessTokenMock.Setup(x => x.AuthenticationState).Returns(AuthenticationState.Anonymous);

            // Act
            Func<Task<object>> action = () => behavior.Handle((object)requestMock.Object, CancellationToken.None, Next);

            // Assert
            var exception = await action.Should().ThrowAsync<NotAuthorizedException>().ConfigureAwait(false);
            exception.And.StatusCode.Should().Be(401);
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task ThrowNotAuthorizedExceptionIfUserIsForbidden()
        {
            // Arrange
            accessTokenMock.Setup(x => x.AuthenticationState).Returns(AuthenticationState.Authenticated);
            accessTokenMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            Func<Task<object>> action = () => behavior.Handle((object)requestMock.Object, CancellationToken.None, Next);

            // Assert
            var exception = await action.Should().ThrowAsync<NotAuthorizedException>().ConfigureAwait(false);
            exception.And.StatusCode.Should().Be(403);
            nextCalled.Should().BeFalse();
        }

        [Fact]
        public async Task AllowAuthorizedActivitiesOnResource()
        {
            // Arrange
            object request = resourceRequestMock.Object;
            string fullName = Guid.NewGuid().ToString();

            accessTokenMock.Setup(x => x.AuthenticationState).Returns(AuthenticationState.Authenticated);
            accessTokenMock.Setup(x => x.IsAuthorized(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            accessTokenMock.Setup(x => x.FullName).Returns(fullName);

            // Act
            var actual = await behavior.Handle(request, CancellationToken.None, Next).ConfigureAwait(false);

            // Assert
            actual.Should().Be(expectedReturn);
            nextCalled.Should().BeTrue();
        }

        private Task<object> Next()
        {
            nextCalled = true;
            return Task.FromResult(expectedReturn);
        }
    }
}
