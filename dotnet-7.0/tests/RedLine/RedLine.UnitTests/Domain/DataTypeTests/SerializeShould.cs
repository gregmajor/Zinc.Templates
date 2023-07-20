using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.DataTypeTests
{
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "It makes the code more clear.")]
    public class SerializeShould
    {
        public static IEnumerable<object[]> SerializeData = new List<object[]>
        {
            new object[] { DataType.Boolean },
            new object[] { DataType.Date },
            new object[] { DataType.Decimal },
            new object[] { DataType.Integer },
            new object[] { DataType.String },
        };

        [Theory]
        [MemberData(nameof(SerializeData))]
        public void OnlySerializeSpecificProperties(DataType expected)
        {
            // Arrange

            // Act
            var json = JsonConvert.SerializeObject(expected);

            // Assert
            var actual = JToken.Parse(json);
            actual.Should().NotBeNull();
            actual.Type.Should().Be(JTokenType.String);
        }

        public static IEnumerable<object[]> SerializeUndefinedData = new List<object[]>
        {
            new object[] { default(DataType) },
        };

        [Theory]
        [MemberData(nameof(SerializeUndefinedData))]
        public void FailToSerializeUnknown(DataType expected)
        {
            // Arrange

            // Act
            var act = () =>
            {
                JsonConvert.SerializeObject(expected);
            };

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [MemberData(nameof(SerializeData))]
        public void RoundTrip(DataType expected)
        {
            // Arrange

            // Act
            var json = JsonConvert.SerializeObject(expected);

            // Assert
            var actual = JsonConvert.DeserializeObject<DataType>(json);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
