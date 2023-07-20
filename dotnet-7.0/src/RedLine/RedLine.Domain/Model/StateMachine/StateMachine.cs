using System;
using System.Collections.Generic;

namespace RedLine.Domain.Model.StateMachine
{
    /// <summary>
    /// A simplified finite state machine.
    /// </summary>
    /// <typeparam name="TStateName">The type of the state name (usually an enum).</typeparam>
    /// <typeparam name="TTransitionName">The type of the reason name (usually an enum).</typeparam>
    public abstract class StateMachine<TStateName, TTransitionName>
    {
        private readonly IClock clock;
        private readonly object transitionLock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine{TStateName,TReasonName}"/> class.
        /// </summary>
        /// <param name="initialStateName">The initial state name.</param>
        /// <param name="initialReasonName">The initial reason name.</param>
        /// <param name="clock">The system clock abstraction.</param>
        protected StateMachine(TStateName initialStateName, TTransitionName initialReasonName, IClock clock)
        {
            this.clock = clock;

            // ReSharper disable once VirtualMemberCallInConstructor
            History = new StateMachineHistory<TStateName, TTransitionName>(clock);

#pragma warning disable S1699
            InitializeStates();
#pragma warning restore S1699

            InitialStateName = initialStateName;
            InitialReasonName = initialReasonName;

#pragma warning disable S1699
            CurrentState = initialStateName;
            CurrentReason = initialReasonName;
#pragma warning restore S1699
        }

        /// <summary>
        /// The name of the resource that owns this state machine.
        /// </summary>
        public abstract string ResourceName { get; }

        /// <summary>
        /// Gets the current reason.
        /// </summary>
        public TTransitionName CurrentReason { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public TStateName CurrentState { get; private set; }

        /// <summary>
        /// Gets or sets the history.
        /// </summary>
        /// <value>The history.</value>
        public StateMachineHistory<TStateName, TTransitionName> History { get; set; }

        /// <summary>
        /// The initial reason name.
        /// </summary>
        protected TTransitionName InitialReasonName { get; }

        /// <summary>
        /// The initial state name.
        /// </summary>
        protected TStateName InitialStateName { get; }

        /// <summary>
        /// The states registered with the state machine.
        /// </summary>
        protected Dictionary<TStateName, FiniteState<TStateName, TTransitionName>> States { get; } = new();

        /// <summary>
        /// Gets the end state of a transition.
        /// </summary>
        /// <param name="transitionName">The name of the transition.</param>
        /// <returns>The end state of a transition.</returns>
        public virtual TStateName GetTransitionEndState(TTransitionName transitionName)
        {
            var currentState = States[CurrentState];
            var transition = currentState.GetTransition(transitionName);
            return transition == null ? default : transition.EndState.Name;
        }

        /// <summary>
        /// Determines whether this instance can transition through the specified transition.
        /// </summary>
        /// <param name="transitionName">Name of the transition.</param>
        /// <returns><c>true</c> if the state machine can transition; <c>false</c> otherwise.</returns>
        protected virtual bool CanTransitionThrough(TTransitionName transitionName)
        {
            return IsValidTransition(CurrentState, transitionName);
        }

        /// <summary>
        /// Determines whether the transition is valid.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="transitionName">The name of the transition.</param>
        /// <returns><c>true</c> if the the transition is valid; <c>false</c> otherwise.</returns>
        protected virtual bool IsValidTransition(TStateName current, TTransitionName transitionName)
        {
            var currentState = States[current];
            return currentState.CanTransition(transitionName);
        }

        /// <summary>
        /// Performs the transition.
        /// </summary>
        /// <param name="transitionName">The name of the transition.</param>
        protected virtual void PerformTransition(TTransitionName transitionName)
        {
            PerformTransition(transitionName, null);
        }

        /// <summary>
        /// Performs the transition.
        /// </summary>
        /// <param name="transitionName">The name of the transition.</param>
        /// <param name="comment">The comment.</param>
        protected virtual void PerformTransition(TTransitionName transitionName, string comment)
        {
            lock (transitionLock)
            {
                if (!IsValidTransition(CurrentState, transitionName))
                {
                    throw new DomainStateTransitionException(ResourceName, CurrentState.ToString(), transitionName.ToString());
                }

                var transition = States[CurrentState].GetTransition(transitionName);

                CurrentState = transition.EndState.Name;
                CurrentReason = transitionName;

                var historyEntry = BuildHistoryEntry(CurrentState, CurrentReason, comment);

                History.AddEntry(historyEntry);
            }
        }

        /// <summary>
        /// Reapplies a transition that has previously occurred.
        /// </summary>
        /// <param name="stateName">The new state.</param>
        /// <param name="reason">The reason for this transition.</param>
        /// <param name="occurredAtDateTime">The date &amp; time this occurred.</param>
        /// <param name="comment">Optional comments about this transition.</param>
        protected virtual void ReapplyTransition(
            TStateName stateName,
            TTransitionName reason,
            DateTimeOffset occurredAtDateTime,
            string comment)
        {
            lock (transitionLock)
            {
                CurrentState = stateName;
                CurrentReason = reason;

                var historyEntry = BuildHistoryEntry(CurrentState, CurrentReason, comment, occurredAtDateTime);

                if (!historyEntry.Equals(History.MostRecentEntry))
                {
                    History.AddEntry(historyEntry);
                }
            }
        }

        /// <summary>
        /// Adds the history entry.
        /// </summary>
        /// <param name="newStateName">New name of the state.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="comment">The comments.</param>
        /// <returns>
        /// A built-up history entry.
        /// </returns>
        protected virtual StateMachineHistoryEntry<TStateName, TTransitionName> BuildHistoryEntry(
            TStateName newStateName,
            TTransitionName reason,
            string comment)
        {
            return BuildHistoryEntry(newStateName, reason, comment, clock.Now());
        }

        /// <summary>
        /// Adds the history entry.
        /// </summary>
        /// <param name="newStateName">New name of the state.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="comment">The comments.</param>
        /// <param name="occurredAtDateTime">The date &amp; time that this transition occurred.</param>
        /// <returns>
        /// A built-up history entry.
        /// </returns>
        protected abstract StateMachineHistoryEntry<TStateName, TTransitionName> BuildHistoryEntry(
            TStateName newStateName,
            TTransitionName reason,
            string comment,
            DateTimeOffset occurredAtDateTime);

        /// <summary>
        /// Initializes the states.
        /// </summary>
        protected abstract void InitializeStates();
    }
}
