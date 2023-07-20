using System;
using FluentAssertions;
using RedLine.A3.Authentication;
using RedLine.Application.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authentication.AuthenticationTokenTests
{
    [Collection(nameof(UnitTestCollection))]
    public class ConstructorShould
    {
        [Fact]
        public void SetUserIdToAnonymousIfNullOrEmpty()
        {
            var token = new AuthenticationToken();
            token.UserId.Should().Be(AuthenticationToken.Anonymous.UserId);
        }

        [Fact]
        public void ThrowNullExceptionIfJwtSecurytTokenIsNull()
        {
            Action act = () => new AuthenticationToken(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowMissingClaimExceptionWhenUpnIsNull()
        {
            Action act = () => TokenMother.CreateToken(null, "userName", "login");
            act.Should().Throw<NotAuthenticatedException>()
                .And.Message.Should().Contain(AuthClaims.Upn);
        }

        [Fact]
        public void ThrowMissingClaimExceptionWhenNameIsNull()
        {
            Action act = () => TokenMother.CreateToken("upn", null, "login");
            act.Should().Throw<NotAuthenticatedException>()
                .And.Message.Should().Contain(AuthClaims.Name);
        }

        [Fact]
        public void ThrowMissingClaimExceptionWhenEmailIsNull()
        {
            Action act = () => TokenMother.CreateToken("upn", "userName", null);
            act.Should().Throw<NotAuthenticatedException>()
                .And.Message.Should().Contain(AuthClaims.Email);
        }
    }
}
