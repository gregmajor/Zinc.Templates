using System;
using FluentAssertions;
using RedLine.Domain.Model.StateMachine;
using Xunit;

namespace RedLine.UnitTests.Domain.Model.StateMachineTests
{
    public class TestStateMachineShould
    {
        [Fact]
        public void ActivateShouldReactivateADeactivatedMachine()
        {
            // Arrange
            var stateMachine = new TestStateMachine();

            // Act
            stateMachine.Deactivate();

            if (stateMachine.CanReactivate)
            {
                stateMachine.Activate();
            }

            // Assert
            stateMachine.CurrentState.Should().Be(TestState.Active);
        }

        [Fact]
        public void BadTransitionShouldThrowDomainStateTransitionException()
        {
            // Arrange
            var stateMachine = new TestStateMachine();

            // Act
            Action action = () => stateMachine.Reactivate();

            // Assert
            action.Should().Throw<DomainStateTransitionException>();
        }

        [Fact]
        public void ActivatedTransitionEndStateShouldBeInactive()
        {
            // Arrange
            var stateMachine = new TestStateMachine();

            // Act
            if (stateMachine.CanDeactivate)
            {
                stateMachine.Deactivate();
            }

            if (stateMachine.CanActivate)
            {
                stateMachine.Activate();
            }

            var transitionEndState = stateMachine.GetTransitionEndState(TestStateReason.Activated);

            // Assert
            transitionEndState.Should().Be(TestState.Active);
        }
    }
}
