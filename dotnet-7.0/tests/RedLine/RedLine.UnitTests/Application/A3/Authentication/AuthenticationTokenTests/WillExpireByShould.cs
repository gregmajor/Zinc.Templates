using System;
using FluentAssertions;
using RedLine.A3.Authentication;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authentication.AuthenticationTokenTests
{
    [Collection(nameof(UnitTestCollection))]
    public class WillExpireByShould
    {
        [Fact]
        public void ReturnTrueIfTheGivenDateIsBeforeJwtValidFrom()
        {
            var token = TokenMother.CreateToken("userId", "userName", "login");
            token.WillExpireBy(DateTime.MinValue).Should().BeTrue();
        }

        [Fact]
        public void ReturnTrueIfTheGivenDateIsAfterJwtValidTo()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt("userId", "userName", "login", DateTime.Now.AddMinutes(1)));
            token.WillExpireBy(DateTime.MaxValue).Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseIfTheGivenDateIsBetweenJwtValidFromAndValidTo()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt("userId", "userName", "login", DateTime.Now.AddMinutes(5)));
            token.WillExpireBy(DateTime.Now).Should().BeFalse();
        }
    }
}
