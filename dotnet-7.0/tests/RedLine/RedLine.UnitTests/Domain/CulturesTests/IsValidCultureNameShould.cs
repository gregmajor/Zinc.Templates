using System.Globalization;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public class IsValidCultureNameShould
    {
        [Theory]
        [ClassData(typeof(ValidCultureNameTestCases))]
        public void ReturnTrueForValidCultureNames(string cultureName)
        {
            Cultures.IsValidCultureName(cultureName).Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(InvalidCultureNameTestCases))]
        public void ReturnFalseForInvalidCultureNames(string cultureName)
        {
            Cultures.IsValidCultureName(cultureName).Should().BeFalse();
        }

        [Fact]
        public void ReturnTrueForTheCurrentCulture()
        {
            Cultures.IsValidCultureName(CultureInfo.CurrentCulture.Name).Should().BeTrue();
        }
    }
}
