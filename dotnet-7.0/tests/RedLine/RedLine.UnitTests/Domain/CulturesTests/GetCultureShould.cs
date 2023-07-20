using System.Globalization;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public class GetCultureShould
    {
        [Theory]
        [ClassData(typeof(ValidCultureNameTestCases))]
        public void ReturnACultureForValidCultureNames(string cultureName)
        {
            Cultures.GetCulture(cultureName).Should().NotBeNull();
        }

        [Theory]
        [ClassData(typeof(InvalidCultureNameTestCases))]
        public void ThrowCultureNotFoundForInvalidCultureNames(string cultureName)
        {
            Assert.Throws<CultureNotFoundException>(() => Cultures.GetCulture(cultureName));
        }

        [Fact]
        public void ReturnTheCurrentCulture()
        {
            Cultures.GetCulture(CultureInfo.CurrentCulture.Name).Should().NotBeNull();
        }
    }
}
