using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.DataTypeTests
{
    public class ConstructorShould
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
        public void CreateWhenValidTypeIsPassed(string typeName)
        {
            // Arrange/Act
            var dataType = (DataType)Activator.CreateInstance(
                typeof(DataType),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object[] { typeName },
                null);

            // Assert
            dataType.Should().BeOfType<DataType>();
            ((string)dataType).Should().Be(typeName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("junk")]
        public void ThrowWhenInvalidTypeIsPassed(string typeName)
        {
            // Arrange
            var act = () =>
            {
                Activator.CreateInstance(
                    typeof(DataType),
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] { typeName },
                    null);
            };

            // Assert
            act.Should().Throw<TargetInvocationException>().And.InnerException.Should().BeOfType<ArgumentException>();
        }
    }
}
