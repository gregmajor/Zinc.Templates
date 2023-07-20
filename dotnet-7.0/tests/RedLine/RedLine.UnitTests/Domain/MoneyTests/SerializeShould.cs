using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.MoneyTests
{
    public class SerializeShould
    {
        public static IEnumerable<object[]> SerializeData = new List<object[]>
        {
            new object[] { 199.999999999M, "en-US" },
            new object[] { 199.99M, "en-CA" },
            new object[] { 199.99M, "fr-CA" },
        };

        [Theory]
        [MemberData(nameof(SerializeData))]
        public void OnlySerializeSpecificProperties(decimal amount, string cultureName)
        {
            // Arrange
            var expected = new Money(amount, cultureName);

            // Act
            var json = JsonConvert.SerializeObject(expected);

            // Assert
            var actual = JToken.Parse(json);
            actual.Should().NotBeNull();
            actual.Children().Count().Should().Be(4);
            actual.Children().Any(c => c.Path == nameof(Money.Amount)).Should().BeTrue();
            actual.Children().Any(c => c.Path == nameof(Money.CultureName)).Should().BeTrue();
            actual.Children().Any(c => c.Path == nameof(Money.CurrencySymbol)).Should().BeTrue();
            actual.Children().Any(c => c.Path == nameof(Money.ISOCurrencyCode)).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(SerializeData))]
        public void RoundTrip(decimal amount, string cultureName)
        {
            // Arrange
            var expected = new Money(amount, cultureName);

            // Act
            var json = JsonConvert.SerializeObject(expected);

            // Assert
            var actual = JsonConvert.DeserializeObject<Money>(json);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
