using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RedLine.A3;
using RedLine.Application.Commands.Grants.RevokeAllGrants;
using RedLine.Application.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Application.Commands.Grants.RevokeAllGrantsTests
{
    public class HandlerShould
    {
        [Fact]
        public async Task ThrowExceptionForSameUser()
        {
            // Arrange
            var userId = "userId";
            var accessToken = new Mock<IAccessToken>();
            accessToken.Setup(a => a.UserId).Returns(userId);
            var handler = new RevokeAllGrantsCommandHandler(null, null);

            var command = new RevokeAllGrantsCommand("Tenant", Guid.NewGuid(), userId, "user name")
            {
                AccessToken = accessToken.Object,
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotAuthorizedException>().WithMessage("Users are not allowed to administer their own grants.").ConfigureAwait(false);
        }
    }
}
