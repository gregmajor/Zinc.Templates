using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.MoneyTests
{
    public class EqualsShould
    {
        [Theory]
        [InlineData("199.99", "en-US")]
        [InlineData("199.3333", "en-US")]
        [InlineData("199.99", "en-CA")]
        [InlineData("199.3333", "en-CA")]
        [InlineData("199.99", "fr-CA")]
        [InlineData("199.3333", "fr-CA")]
        public void ReturnTrueWhenEqual(string amount, string culture)
        {
            var expected = new Money(decimal.Parse(amount), culture);
            var actual = new Money(decimal.Parse(amount), culture);

            actual.Equals(expected).Should().BeTrue();
            (actual == expected).Should().BeTrue();
            (actual != expected).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 3, "0.3333333333333333333333333333")]
        [InlineData(100000000, 3, "33333333.333333333333333333333")]
        public void ReturnTrueWhenAmountHasMaxDecimals(int dividend, int divisor, string expectedValue)
        {
            var expected = new Money(decimal.Parse(expectedValue));
            var actual = new Money(decimal.Divide(new decimal(dividend), new decimal(divisor)));

            actual.Equals(expected).Should().BeTrue();
            (actual == expected).Should().BeTrue();
            (actual != expected).Should().BeFalse();
        }

        [Theory]
        [InlineData("199.99", "en-US")]
        [InlineData("199.9945", "en-US")]
        public void ReturnFalseWhenAmountsDiffer(string amount, string culture)
        {
            var expected = new Money(decimal.Parse(amount), culture);
            var actual = new Money(199.9950M, culture);

            actual.Equals(expected).Should().BeFalse();
            (actual == expected).Should().BeFalse();
            (actual != expected).Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseIfCurrencyCodesDiffer()
        {
            var amount = 199.99m;
            var expected = new Money(amount, "en-US");
            var actual = new Money(amount, "en-CA");

            actual.Equals(expected).Should().BeFalse();
            (actual == expected).Should().BeFalse();
            (actual != expected).Should().BeTrue();
        }

        [Fact]
        public void ReturnTrueIfCurrencyCodesMatch()
        {
            var amount = 199.99m;
            var expected = new Money(amount, "en-CA");
            var actual = new Money(amount, "fr-CA");

            actual.Equals(expected).Should().BeTrue();
            (actual == expected).Should().BeTrue();
            (actual != expected).Should().BeFalse();
        }
    }
}
