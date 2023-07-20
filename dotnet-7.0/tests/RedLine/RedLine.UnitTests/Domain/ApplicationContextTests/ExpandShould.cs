using System;
using FluentAssertions;
using RedLine.Domain;
using RedLine.Domain.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Domain.ApplicationContextTests
{
    [Collection(nameof(UnitTestCollection))]
    public class ExpandShould
    {
        [Fact]
        public void ExpandValuesUsingContext()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Expand("pass: %key1%");

            // Assert
            result.Should().Be("pass: value1");
        }

        [Fact]
        public void ExpandEnvironmentVariables()
        {
            // Arrange
            var env1 = "test-env-var";
            Environment.SetEnvironmentVariable("env1", env1);

            // Act
            var result = ApplicationContext.Expand("key1: %key1%, env1: %env1%");

            // Assert
            result.Should().Be($"key1: value1, env1: {env1}");
        }

        [Fact]
        public void ReturnNullForNonExistingValue()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Expand("%non-existing%");

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void RecursivelyExpandValues()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Expand("expanding: %key3%");

            // Assert
            result.Should().Be("expanding: value3: 'value2: 'value1''");
        }

        [Fact]
        public void ThrowInvalidConfigurationExceptionWhenCircularReferenceDetected()
        {
            // Arrange
            // Act
            Action act = () => ApplicationContext.Expand("expanding: %key5%");

            // Assert
            act.Should().Throw<InvalidConfigurationException>();
        }
    }
}
