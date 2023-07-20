using System;
using System.Collections.Generic;
using FluentAssertions;
using RedLine.Domain;
using RedLine.Domain.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Domain.ApplicationContextTests
{
    [Collection(nameof(UnitTestCollection))]
    public class GetShould
    {
        private readonly UnitTestFixture fixture;

        public GetShould(UnitTestFixture fixture)
        {
            this.fixture = fixture;
        }

        public static IEnumerable<object[]> GetTimeSpanEntries()
        {
            yield return new object[] { "00:00:10", TimeSpan.FromSeconds(10) };
            yield return new object[] { "00:05:00", TimeSpan.FromMinutes(5) };
            yield return new object[] { "00:00:00.039", TimeSpan.FromMilliseconds(39) };
            yield return new object[] { "04:00:00", TimeSpan.FromHours(4) };
            yield return new object[] { null, TimeSpan.FromMinutes(5) };
        }

        [Fact]
        public void ReturnExistingValue()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Get("key1");

            // Assert
            result.Should().Be("value1");
        }

        [Fact]
        public void ThrowInvalidConfigurationExceptionForNonExistingValue()
        {
            // Arrange
            // Act
            Action action = () => ApplicationContext.Get("non-existing");

            // Assert
            action.Should().Throw<InvalidConfigurationException>();
        }

        [Fact]
        public void ExpandValuesDelimitedWithPercentCharacters()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Get("key2");

            // Assert
            result.Should().Be("value2: 'value1'");
        }

        [Fact]
        public void RecursivelyExpandValues()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Get("key3");

            // Assert
            result.Should().Be("value3: 'value2: 'value1''");
        }

        [Fact]
        public void ThrowInvalidConfigurationExceptionWhenCircularReferenceDetected()
        {
            // Arrange
            // Act
            Action act = () => ApplicationContext.Get("key5");

            // Assert
            act.Should().Throw<InvalidConfigurationException>();
        }

        [Theory]
        [MemberData(nameof(GetTimeSpanEntries))]
        public void ReturnAllowedClockSkewTimeSpan(string configText, TimeSpan expected)
        {
            // Arrange
            var d = new Dictionary<string, string>(fixture.ContextValues);

            if (configText == null)
            {
                d.Remove(nameof(ApplicationContext.AllowedClockSkew));
            }
            else
            {
                d[nameof(ApplicationContext.AllowedClockSkew)] = configText;
            }

            ApplicationContext.Init(d);

            // Act
            var clockSkew = ApplicationContext.AllowedClockSkew;

            // Assert
            clockSkew.Should().Be(expected);
        }
    }
}
