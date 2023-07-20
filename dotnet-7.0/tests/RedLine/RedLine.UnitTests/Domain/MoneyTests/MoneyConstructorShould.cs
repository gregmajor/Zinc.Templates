using System;
using System.Globalization;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.MoneyTests
{
    public class MoneyConstructorShould
    {
        [Fact]
        public void SetTheProperties()
        {
            const decimal amount = 100.99m;
            const string cultureName = "fr-CA";

            var money = new Money(amount, cultureName);

            money.Amount.Should().Be(amount);
            money.CultureName.Should().Be(cultureName);

            var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
            var regionInfo = new RegionInfo(cultureInfo.Name);
            money.CurrencySymbol.Should().Be(cultureInfo.NumberFormat.CurrencySymbol);
            money.ISOCurrencyCode.Should().Be(regionInfo.ISOCurrencySymbol);
        }

        [Fact]
        public void SetTheCurrentCulture()
        {
            new Money(100.99m).CultureName.Should().Be("en-US");
        }

        [Fact]
        public void ThrowIfCultureInfoIsNull()
        {
            Action action = () => new Money(100m, (CultureInfo)null);
            action.Should().Throw<ArgumentNullException>().And.Message.Contains("culture");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void ThrowWhenCultureNameIsNullOrWhitespace(string cultureName)
        {
            Action action = () => new Money(100m, cultureName);
            action.Should().Throw<ArgumentException>().And.ParamName.Contains("cultureName");
        }

        [Fact]
        public void ThrowWhenCultureIsNotFound()
        {
            Action action = () => new Money(100m, "xx-AB");
            action.Should().Throw<CultureNotFoundException>();
        }
    }
}
