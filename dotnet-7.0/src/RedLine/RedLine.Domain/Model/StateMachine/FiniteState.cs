using System;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.Domain.Model.StateMachine
{
    /// <summary>
    /// A simplified finite state.
    /// </summary>
    /// <typeparam name="TStateName">The type of the state name (usually an enum).</typeparam>
    /// <typeparam name="TTransitionName">The type of the transition name (usually an enum).</typeparam>
    public class FiniteState<TStateName, TTransitionName>
    {
        private readonly Dictionary<TTransitionName, StateTransition<TStateName, TTransitionName>> transitions = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiniteState{TStateName, TTransitionName}"/> class.
        /// </summary>
        /// <param name="stateName">The status.</param>
        public FiniteState(TStateName stateName)
        {
            Name = stateName;
        }

        /// <summary>
        /// Gets or sets the name of the state.
        /// </summary>
        /// <value>
        /// The state name.
        /// </value>
        public TStateName Name { get; }

        /// <summary>
        /// The transitions available beginning at this state.
        /// </summary>
        public IEnumerable<TTransitionName> ValidTransitions => transitions.Values
            .Where(t => CanTransition(t.TransitionName))
            .Select(t => t.TransitionName);

        /// <summary>
        /// Adds a transition from this state to another.
        /// </summary>
        /// <param name="transitionName">The name of the transition.</param>
        /// <param name="endState">The ending state of the transition.</param>
        public void AddTransition(TTransitionName transitionName, FiniteState<TStateName, TTransitionName> endState)
        {
            var stateTransition = new StateTransition<TStateName, TTransitionName>(this, transitionName, endState);
            AddTransition(stateTransition);
        }

        /// <summary>
        /// Adds a transition from this state to another.
        /// </summary>
        /// <param name="transitionName"><inheritdoc cref="StateTransition{TStateName,TTransitionName}.TransitionName"/></param>
        /// <param name="endState"><inheritdoc cref="StateTransition{TStateName,TTransitionName}.EndState"/></param>
        /// <param name="guard"><inheritdoc cref="StateTransition{TStateName,TTransitionName}.Guard"/></param>
        public void AddTransition(TTransitionName transitionName, FiniteState<TStateName, TTransitionName> endState, Func<bool> guard)
        {
            var stateTransition = new StateTransition<TStateName, TTransitionName>(this, transitionName, endState, guard);
            AddTransition(stateTransition);
        }

        /// <summary>
        /// Determines if it is valid to transition from this state through the specified transition.
        /// </summary>
        /// <param name="transitionName">The name of the transition.</param>
        /// <returns><c>true</c> if the transition is valid. <c>false</c> otherwise.</returns>
        public bool CanTransition(TTransitionName transitionName)
        {
            return transitions.ContainsKey(transitionName) &&
                   transitions[transitionName].Guard();
        }

        /// <summary>
        /// Gets the transition.
        /// </summary>
        /// <param name="transitionName">The name of the transition.</param>
        /// <returns>The state transition corresponding to the specified name.</returns>
        public StateTransition<TStateName, TTransitionName> GetTransition(TTransitionName transitionName)
        {
            if (transitions.ContainsKey(transitionName))
            {
                return transitions[transitionName];
            }

            return null;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Comparer.Instance.Equals(this, obj as FiniteState<TStateName, TTransitionName>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Comparer.Instance.GetHashCode(this);
        }

        private void AddTransition(StateTransition<TStateName, TTransitionName> transition)
        {
            if (!Equals(transition.BeginState, this))
            {
                throw new ArgumentException($"{nameof(transition.BeginState)} must be {Name}");
            }

            transitions[transition.TransitionName] = transition;
        }

        /// <summary>
        /// Comparer for <see cref="FiniteState{TStateName,TTransitionName}"/>.
        /// </summary>
        public class Comparer : IEqualityComparer<FiniteState<TStateName, TTransitionName>>
        {
            /// <summary>
            /// The default instance.
            /// </summary>
            public static readonly Comparer Instance = new Comparer();

            /// <summary>
            /// Singleton.
            /// </summary>
            private Comparer()
            {
            }

            /// <summary>
            /// Compares two instances of <see cref="FiniteState{TStateName,TTransitionName}"/>.
            /// </summary>
            /// <param name="x">The first instance.</param>
            /// <param name="y">The second instance.</param>
            /// <returns><c>true</c> if the two instances are equivalent. Otherwise, <c>false</c>.</returns>
            public bool Equals(FiniteState<TStateName, TTransitionName> x, FiniteState<TStateName, TTransitionName> y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return EqualityComparer<TStateName>.Default.Equals(x.Name, y.Name);
            }

            /// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)"/>
            public int GetHashCode(FiniteState<TStateName, TTransitionName> obj)
            {
                return EqualityComparer<TStateName>.Default.GetHashCode(obj.Name);
            }
        }
    }
}
