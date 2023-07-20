using System;
using FluentAssertions;
using RedLine.A3.Authentication;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authentication.AuthenticationTokenTests
{
    [Collection(nameof(UnitTestCollection))]
    public class AuthenticationStateShould
    {
        [Fact]
        public void BeAuthenticatedIfJwtIsNotExpired()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt("userId", "userName", "login", DateTime.Now.AddHours(1)));
            token.AuthenticationState.Should().Be(AuthenticationState.Authenticated);
        }

        [Fact]
        public void BeAnonymousIfJwtIsExpired()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt("userId", "userName", "login", DateTime.Now.AddHours(-1)));
            token.AuthenticationState.Should().Be(AuthenticationState.Anonymous);
        }
    }
}
