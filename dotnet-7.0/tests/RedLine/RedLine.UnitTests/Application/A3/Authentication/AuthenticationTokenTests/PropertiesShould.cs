using System;
using FluentAssertions;
using RedLine.A3.Authentication;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authentication.AuthenticationTokenTests
{
    [Collection(nameof(UnitTestCollection))]
    public class PropertiesShould
    {
        [Fact]
        public void ReturnFirstName()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt(
                "subect@issuer",
                "Joe User",
                "joeuser@redline.services",
                "Joe",
                "User",
                DateTime.UtcNow.AddHours(4)));

            token.FirstName.Should().Be("Joe");
        }

        [Fact]
        public void ReturnLastName()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt(
                "subect@issuer",
                "Joe User",
                "joeuser@redline.services",
                "Joe",
                "User",
                DateTime.UtcNow.AddHours(4)));

            token.LastName.Should().Be("User");
        }

        [Fact]
        public void ReturnFullName()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt(
                "subect@issuer",
                "Joe User",
                "joeuser@redline.services",
                "Joe",
                "User",
                DateTime.UtcNow.AddHours(4)));

            token.FullName.Should().Be("Joe User");
        }

        [Fact]
        public void ReturnLogin()
        {
            var token = new AuthenticationToken(TokenMother.CreateJwt(
                "subect@issuer",
                "Joe User",
                "joeuser@redline.services",
                "Joe",
                "User",
                DateTime.UtcNow.AddHours(4)));

            token.Login.Should().Be("joeuser@redline.services");
        }
    }
}
