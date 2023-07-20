using System;
using System.Collections.Generic;

namespace RedLine.Domain.Model.StateMachine
{
    /// <summary>
    /// A transition between <see cref="FiniteState{TStateName, TTransitionName}"/>s.
    /// </summary>
    /// <typeparam name="TStateName">The type of the state name (usually an enum).</typeparam>
    /// <typeparam name="TTransitionName">The type of transition name (usually an enum).</typeparam>
    public class StateTransition<TStateName, TTransitionName>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="StateTransition{TStateName,TTransitionName}"/>.
        /// </summary>
        /// <param name="beginState"><inheritdoc cref="BeginState"/></param>
        /// <param name="transitionName"><inheritdoc cref="TransitionName"/></param>
        /// <param name="endState"><inheritdoc cref="EndState"/></param>
        public StateTransition(
            FiniteState<TStateName, TTransitionName> beginState,
            TTransitionName transitionName,
            FiniteState<TStateName, TTransitionName> endState)
        {
            BeginState = beginState ?? throw new ArgumentNullException(nameof(beginState));
            TransitionName = transitionName ?? throw new ArgumentNullException(nameof(transitionName));
            EndState = endState ?? throw new ArgumentNullException(nameof(endState));
            Guard = () => true;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="StateTransition{TStateName,TTransitionName}"/>.
        /// </summary>
        /// <param name="beginState"><inheritdoc cref="BeginState"/></param>
        /// <param name="transitionName"><inheritdoc cref="TransitionName"/></param>
        /// <param name="endState"><inheritdoc cref="EndState"/></param>
        /// <param name="guard"><inheritdoc cref="Guard"/></param>
        public StateTransition(
            FiniteState<TStateName, TTransitionName> beginState,
            TTransitionName transitionName,
            FiniteState<TStateName, TTransitionName> endState,
            Func<bool> guard)
        {
            BeginState = beginState ?? throw new ArgumentNullException(nameof(beginState));
            TransitionName = transitionName ?? throw new ArgumentNullException(nameof(transitionName));
            EndState = endState ?? throw new ArgumentNullException(nameof(endState));
            Guard = guard ?? (() => true);
        }

        /// <summary>
        /// The beginning state of this transition.
        /// </summary>
        public FiniteState<TStateName, TTransitionName> BeginState { get; }

        /// <summary>
        /// The name of this transition.
        /// </summary>
        public TTransitionName TransitionName { get; }

        /// <summary>
        /// The ending state of this transition.
        /// </summary>
        public FiniteState<TStateName, TTransitionName> EndState { get; }

        /// <summary>
        /// Guard to check additional conditions when a transition is valid.
        /// </summary>
        public Func<bool> Guard { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Comparer.Instance.Equals(this, obj as StateTransition<TStateName, TTransitionName>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Comparer.Instance.GetHashCode(this);
        }

        /// <summary>
        /// Equality comparer for <see cref="StateTransition{TStateName,TTransitionName}"/>.
        /// </summary>
        public class Comparer : IEqualityComparer<StateTransition<TStateName, TTransitionName>>
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
            /// Compares two instances of <see cref="StateTransition{TStateName,TTransitionName}"/>.
            /// </summary>
            /// <param name="x">The first instance.</param>
            /// <param name="y">The second instance.</param>
            /// <returns><c>true</c> if the two instances are equivalent. Otherwise, <c>false</c>.</returns>
            public bool Equals(StateTransition<TStateName, TTransitionName> x, StateTransition<TStateName, TTransitionName> y)
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

                return Equals(x.BeginState, y.BeginState) && EqualityComparer<TTransitionName>.Default.Equals(x.TransitionName, y.TransitionName);
            }

            /// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)"/>
            public int GetHashCode(StateTransition<TStateName, TTransitionName> obj)
            {
                return HashCode.Combine(obj.BeginState, obj.TransitionName);
            }
        }
    }
}
