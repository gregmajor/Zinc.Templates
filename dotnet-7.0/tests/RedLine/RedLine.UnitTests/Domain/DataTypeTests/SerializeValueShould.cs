using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Domain.DataTypeTests
{
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "It makes the code more clear.")]
    public class SerializeValueShould
    {
        public static IEnumerable<object[]> ValidData
        {
            get
            {
                var dateTimeOffset = DateTimeOffset.Now;
                var dateOnly = DateOnly.FromDateTime(DateTimeOffset.Now.Date);

                return new List<object[]>
                {
                    new object[] { DataType.Boolean, true, "true" },
                    new object[] { DataType.Boolean, false, "false" },
                    new object[] { DataType.Date, dateTimeOffset, dateTimeOffset.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture) },
                    new object[] { DataType.Date, dateOnly, dateOnly.ToString("o", CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, 123.456M, 123.456M.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, 123.456F, 123.456F.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, 123.456D, 123.456D.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, 123456L, 123456L.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, 123456, 123456.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, (short)12345, ((short)12345).ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Decimal, (byte)123, ((byte)123).ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Integer, 123456L, 123456L.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Integer, 123456, 123456.ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Integer, (short)12345, ((short)12345).ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.Integer, (byte)123, ((byte)123).ToString(CultureInfo.InvariantCulture) },
                    new object[] { DataType.String, "123456", "123456" },
                    new object[] { DataType.Boolean, null, null },
                    new object[] { DataType.Date, null, null },
                    new object[] { DataType.Decimal, null, null },
                    new object[] { DataType.Integer, null, null },
                    new object[] { DataType.String, null, null },
                };
            }
        }

        [Theory]
        [MemberData(nameof(ValidData))]
        public void SerializeValidTypes(DataType dataType, object value, string expected)
        {
            // Arrange / Act
            var actual = dataType.SerializeValue(value);

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ValidData))]
        public void DeserializeValidTypes(DataType dataType, object expected, string value)
        {
            // Arrange / Act
            var actual = dataType.DeserializeValue(value);

            // Assert
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(ValidData))]
        [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "The data is used by multiple tests.")]
        public void RoundTrip(DataType dataType, object expected, string ignore)
        {
            // Arrange / Act
            var s = dataType.SerializeValue(expected);
            var actual = dataType.DeserializeValue(s);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> InvalidSerializeData =
            new List<object[]>
            {
                new object[] { DataType.Boolean, 123 },
                new object[] { DataType.Date, 123 },
                new object[] { DataType.Decimal, "abcd" },
                new object[] { DataType.Integer, "abcd" },
                new object[] { DataType.String, 123 },
            };

        [Theory]
        [MemberData(nameof(InvalidSerializeData))]
        public void ThrowWhenSerializingInvalidTypes(DataType dataType, object value)
        {
            // Arrange / Act
            var act = () => dataType.SerializeValue(value);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        public static IEnumerable<object[]> InvalidDeserializeData =
            new List<object[]>
            {
                new object[] { DataType.Boolean, 123 },
                new object[] { DataType.Date, 123 },
                new object[] { DataType.Decimal, "abcd" },
                new object[] { DataType.Integer, "abcd" },
            };

        [Theory]
        [MemberData(nameof(InvalidDeserializeData))]
        public void ThrowWhenDeserializingInvalidTypes(DataType dataType, object value)
        {
            // Arrange / Act
            var act = () => dataType.DeserializeValue(value.ToString());

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
