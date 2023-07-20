using System;
using FluentAssertions;
using RedLine.Domain;
using RedLine.Extensions.Testing;
using Xunit;

namespace RedLine.UnitTests.Testing.Equivalency
{
    [Collection(nameof(UnitTestCollection))]
    public class MoneyEquivalencyShould
    {
        [Theory]
        [InlineData("1000.77777", "en-US")]
        [InlineData("1000.11111", "en-US")]
        [InlineData("1000.77777", "en-CA")]
        [InlineData("1000.11111", "en-CA")]
        [InlineData("1000.77777", "fr-CA")]
        [InlineData("1000.11111", "fr-CA")]
        public void MatchEquivalentMoneys(string amount, string culture)
        {
            // Arrange
            var expected = new Money(decimal.Parse(amount), culture);
            var actual = new Money(decimal.Round(expected.Amount, 4, MidpointRounding.AwayFromZero), culture);

            // Act/Assert
            actual.Should().NotBe(expected);
            actual.Should().BeEquivalentTo(expected, options => options.UseMoneyEquivalency(4));
        }

        [Theory]
        [InlineData("1000.77", "en-US")]
        [InlineData("1000.777", "en-US")]
        public void NotMatchEqualMoneys(string amount, string culture)
        {
            // Arrange
            var expected = new Money(decimal.Parse(amount), culture);
            var actual = new Money(decimal.Round(expected.Amount, 4, MidpointRounding.AwayFromZero), culture);

            // Act/Assert
            actual.Should().Be(expected);
            actual.Should().BeEquivalentTo(expected, options => options.UseMoneyEquivalency(4));
        }

        [Fact]
        public void NotMatchNonEquivalentMoneys()
        {
            // Arrange
            var expected = new Money(1000.77m, "en-US");
            var actual = new Money(1000.66m, "en-US");

            // Act/Assert
            actual.Should().NotBe(expected);
            actual.Should().NotBeEquivalentTo(expected, options => options.UseMoneyEquivalency(4));
        }
    }
}
