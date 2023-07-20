using System.Globalization;
using System.Linq;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public class NamesShould
    {
        [Theory]
        [ClassData(typeof(ValidCultureNameTestCases))]
        public void ContainSeveralSpecificCultureNames(string cultureName)
        {
            Cultures.Names.Should().Contain(cultureName);
        }

        [Theory]
        [ClassData(typeof(InvalidCultureNameTestCases))]
        public void NotContainInvalidCultureNames(string cultureName)
        {
            if (cultureName == null)
            {
                Cultures.Names.Any(x => x == null).Should().BeFalse();
                return;
            }

            Cultures.Names.Should().NotContain(cultureName);
        }

        [Fact]
        public void ContainTheCurrentCulture()
        {
            Cultures.Names.Should().Contain(CultureInfo.CurrentCulture.Name);
        }
    }
}
