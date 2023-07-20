using System;
using System.Collections.Generic;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.DataTypeTests
{
    public class ParseShould
    {
        public static IEnumerable<object[]> CreateData = new List<object[]>
        {
            new object[] { DataType.Boolean },
            new object[] { DataType.Date },
            new object[] { DataType.Decimal },
            new object[] { DataType.Integer },
            new object[] { DataType.String },
        };

        [Theory]
        [MemberData(nameof(CreateData))]
        public void ParseValidTypes(string typeName)
        {
            // Arrange/Act
            var dataType = DataType.Parse(typeName);

            // Assert
            dataType.Should().BeOfType<DataType>();
            ((string)dataType).Should().Be(typeName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("junk")]
        public void ThrowWhenInvalidTypeIsPassed(string typeName)
        {
            // Arrange
            var act = () =>
            {
                DataType.Parse(typeName);
            };

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}
