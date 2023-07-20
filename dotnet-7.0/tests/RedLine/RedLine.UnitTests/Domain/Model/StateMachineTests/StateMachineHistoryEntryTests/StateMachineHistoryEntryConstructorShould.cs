using System;
using FluentAssertions;
using RedLine.Domain.Model;
using RedLine.Domain.Model.StateMachine;
using Xunit;

namespace RedLine.UnitTests.Domain.Model.StateMachineTests.StateMachineHistoryEntryTests
{
    public class StateMachineHistoryEntryConstructorShould
    {
        [Fact]
        public void NullClockShouldThrowArgumentNullException()
        {
            // Arrange
            IClock clock = null;

            //Act
            Action testCode = () => new StateMachineHistoryEntry<int, int>(0, 0, 0, "A comment", clock);

            testCode.Should().Throw<ArgumentNullException>();
        }
    }
}
