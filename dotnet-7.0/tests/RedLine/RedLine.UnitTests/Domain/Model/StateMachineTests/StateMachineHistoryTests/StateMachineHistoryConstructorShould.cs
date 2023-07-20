using System;
using FluentAssertions;
using RedLine.Domain.Model;
using RedLine.Domain.Model.StateMachine;

namespace RedLine.UnitTests.Domain.Model.StateMachineTests.StateMachineHistoryTests
{
    public class StateMachineHistoryConstructorShould
    {
        public void NullClockShouldThrowArgumentNullException()
        {
            // Arrange
            IClock clock = null;

            // Act
            Action testCode = () => new StateMachineHistory<int, int>(clock);

            // Assert
            testCode.Should().Throw<ArgumentNullException>();
        }
    }
}
