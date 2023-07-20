using System;
using System.Linq;
using RedLine.Domain.Model;
using RedLine.Domain.Model.StateMachine;

namespace RedLine.UnitTests.Domain.Model.StateMachineTests
{
    /// <summary>
    /// The finite state machine for an account calendar.
    /// </summary>
    public class TestStateMachine : StateMachine<TestState, TestStateReason>
    {
        /// <summary>
        /// Creates an instance of the account calendar state.
        /// </summary>
        /// <param name="clock">The system clock abstraction.</param>
        public TestStateMachine(IClock clock)
            : base(TestState.Active, TestStateReason.Activated, clock)
        { }

        /// <summary>
        /// Creates an instance of the account calendar state.
        /// </summary>
        public TestStateMachine()
            : base(TestState.Active, TestStateReason.Activated, new Clock())
        { }

        /// <inheritdoc cref="StateMachine{TStateName,TTransitionName}.ResourceName"/>
        public override string ResourceName => "Test State Machine";

        /// <summary>
        /// Gets a value indicating whether the account calendar can be activated.
        /// </summary>
        public bool CanActivate => CanTransitionThrough(TestStateReason.Activated);

        /// <summary>
        /// Gets a value indicating whether the account calendar can be deactivated.
        /// </summary>
        public bool CanDeactivate => CanTransitionThrough(TestStateReason.Deactivated);

        /// <summary>
        /// Gets a value indicating whether the account calendar can be reactivated.
        /// </summary>
        public bool CanReactivate => CanTransitionThrough(TestStateReason.Reactivated);

        /// <summary>
        /// Gets a value indicating whether the account calendar is active.
        /// </summary>
        public bool IsActive => CurrentState == TestState.Active;

        /// <summary>
        /// Gets a value indicating whether the account calendar is inactive.
        /// </summary>
        public bool IsInactive => CurrentState == TestState.Inactive;

        /// <summary>
        /// Gets a value indicating whether the account calendar was reactivated.
        /// </summary>
        public bool IsReactivated => CurrentState == TestState.Active && CurrentReason == TestStateReason.Reactivated;

        /// <summary>
        /// Gets a value indicating whether the account calendar was ever deactivated.
        /// </summary>
        public bool WasEverDeactivated => History.Entries.Any(x => x.StateName == TestState.Inactive);

        /// <summary>
        /// Activates the account calendar.
        /// </summary>
        public void Activate()
        {
            Activate(null);
        }

        /// <summary>
        /// Activates the account calendar.
        /// </summary>
        /// <param name="comment">Comments associated with the activation.</param>
        public void Activate(string comment)
        {
            if (WasEverDeactivated)
            {
                Reactivate(comment);
                return;
            }

            PerformTransition(TestStateReason.Activated, comment);
        }

        /// <summary>
        /// Deactivates the account calendar.
        /// </summary>
        public void Deactivate()
        {
            Deactivate(null);
        }

        /// <summary>
        /// Deactivates the account calendar.
        /// </summary>
        /// <param name="comment">Comments associated with the deactivation.</param>
        public void Deactivate(string comment)
        {
            PerformTransition(TestStateReason.Deactivated, comment);
        }

        /// <summary>
        /// Reactivates the account calendar.
        /// </summary>
        public void Reactivate()
        {
            Reactivate(null);
        }

        /// <summary>
        /// Reactivates the account calendar.
        /// </summary>
        /// <param name="comment">Comments associated with the reactivation.</param>
        public void Reactivate(string comment)
        {
            PerformTransition(TestStateReason.Reactivated, comment);
        }

        /// <inheritdoc/>>
        protected override StateMachineHistoryEntry<TestState, TestStateReason> BuildHistoryEntry(TestState newStateName, TestStateReason reason, string comment, DateTimeOffset occurredAtDateTime)
        {
            var entryNumber = 1;

            if (History.Entries.Any())
            {
                entryNumber = History.Entries.Max(x => x.EntryNumber) + 1;
            }

            var historyEntry = new StateMachineHistoryEntry<TestState, TestStateReason>(
                entryNumber,
                newStateName,
                reason,
                comment,
                occurredAtDateTime);

            return historyEntry;
        }

        /// <summary>
        /// Initializes the states.
        /// </summary>
        protected override void InitializeStates()
        {
            var active = new FiniteState<TestState, TestStateReason>(TestState.Active);
            var inactive = new FiniteState<TestState, TestStateReason>(TestState.Inactive);

            active.AddTransition(TestStateReason.Deactivated, inactive);
            inactive.AddTransition(TestStateReason.Activated, active, () => !WasEverDeactivated);
            inactive.AddTransition(TestStateReason.Reactivated, active, () => WasEverDeactivated);

            States.Add(TestState.Active, active);
            States.Add(TestState.Inactive, inactive);
        }
    }    
}
