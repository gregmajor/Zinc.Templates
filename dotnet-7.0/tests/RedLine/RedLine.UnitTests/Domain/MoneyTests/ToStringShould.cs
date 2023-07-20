using System.Globalization;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.MoneyTests
{
    public class ToStringShould
    {
        [Theory]
        [InlineData("en-US")]
        [InlineData("en-CA")]
        [InlineData("fr-CA")]
        public void NotContainCurrencyCode(string cultureName)
        {
            var money = new Money(1199.99m, cultureName);
            var actual = money.ToString();

            actual.Should().NotContain(money.CurrencySymbol);
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("en-CA")]
        [InlineData("fr-CA")]
        public void HonorCultureSpecificFormatting(string cultureName)
        {
            // Arrange
            const decimal amount = 1199.99m;
            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

            // Act
            var money = new Money(amount, cultureName);
            var actual = money.ToString();
            var expected = string.Format(cultureInfo, "{0:C}", amount)
                .Replace(money.CurrencySymbol, string.Empty)
                .Trim();

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

            // Act
            var money = new Money(amount, cultureName);
            var actual = money.ToString();
            var expected = string.Format(cultureInfo, "{0:C}", amount)
                .Replace(money.CurrencySymbol, string.Empty)
                .Trim();

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

            // Act
            var money = new Money(amount, cultureName);
            var actual = money.ToString();
            var expected = string.Format(cultureInfo, "{0:C}", amount)
                .Replace(money.CurrencySymbol, string.Empty)
                .Trim();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
