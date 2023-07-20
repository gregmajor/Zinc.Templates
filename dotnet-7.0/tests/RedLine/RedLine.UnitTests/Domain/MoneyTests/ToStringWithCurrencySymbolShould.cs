using System.Globalization;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.MoneyTests
{
    public class ToStringWithCurrencySymbolShould
    {
        [Theory]
        [InlineData("en-US")]
        [InlineData("en-CA")]
        [InlineData("fr-CA")]
        public void HonorCultureSpecificFormatting(string cultureName)
        {
            // Arrange
            const decimal amount = 1199.99m;
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var expected = string.Format(cultureInfo, "{0:C}", amount);

            // Act
            var actual = new Money(amount, cultureName).ToStringWithCurrencySymbol();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("en-CA")]
        [InlineData("fr-CA")]
        public void RoundDownAccordingToCulture(string cultureName)
        {
            // Arrange
            const decimal amount = 1199.333333333333333333m;
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var expected = string.Format(cultureInfo, "{0:C}", amount);

            // Act
            var actual = new Money(amount, cultureName).ToStringWithCurrencySymbol();

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("en-CA")]
        [InlineData("fr-CA")]
        public void RoundUpAccordingToCulture(string cultureName)
        {
            // Arrange
            const decimal amount = 1199.3353333333333333333m;
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var expected = string.Format(cultureInfo, "{0:C}", amount);

            // Act
            var actual = new Money(amount, cultureName).ToStringWithCurrencySymbol();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
