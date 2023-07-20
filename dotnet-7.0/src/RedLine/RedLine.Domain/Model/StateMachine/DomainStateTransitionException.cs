using System;
using System.Runtime.Serialization;
using RedLine.Domain.Exceptions;

namespace RedLine.Domain.Model.StateMachine
{
    /// <summary>
    /// A state machine in the domain could not perform the requested transition.
    /// </summary>
    [Serializable]
    public class DomainStateTransitionException : DomainException
    {
        /// <summary>
        /// Initializes aa new instance of <see cref="DomainStateTransitionException"/>.
        /// </summary>
        /// <param name="resourceName">The name of the resource being transitioned.</param>
        /// <param name="currentStateName">The name of the state machine's current state.</param>
        /// <param name="transitionName">The name of the specified transition.</param>
        public DomainStateTransitionException(string resourceName, string currentStateName, string transitionName)
            : base(CreateMessage(resourceName, currentStateName, transitionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainStateTransitionException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected DomainStateTransitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string CreateMessage(string resourceName, string currentStateName, string transitionName)
        {
            return $"{resourceName} cannot be {transitionName.ToLowerInvariant()} when it is {currentStateName.ToLowerInvariant()}.";
        }
    }
}
