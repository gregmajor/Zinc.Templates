using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RedLine.Domain.Model.StateMachine
{
    /// <summary>
    /// The simple finite state machine history entry.
    /// </summary>
    /// <typeparam name="TStateName">The state name type.</typeparam>
    /// <typeparam name="TReasonName">The reason name type.</typeparam>
    public sealed class StateMachineHistoryEntry<TStateName, TReasonName> : IEquatable<StateMachineHistoryEntry<TStateName, TReasonName>>
    {
        /// <summary>
        /// Initializes a new instance of the simple finite state machine history entry.
        /// </summary>
        /// <param name="entryNumber">The entry number.</param>
        /// <param name="stateName">The state name.</param>
        /// <param name="reasonName">The reason name.</param>
        /// <param name="comments">The comments.</param>
        /// <param name="occurredAtDateTime">The date &amp; time this transition occurred.</param>
        [JsonConstructor]
        public StateMachineHistoryEntry(
            int entryNumber,
            TStateName stateName,
            TReasonName reasonName,
            string comments,
            DateTimeOffset occurredAtDateTime)
        {
            Sid = Guid.NewGuid();
            Comments = comments;

            EntryNumber = entryNumber;
            StateName = stateName;
            ReasonName = reasonName;

            EntryDate = occurredAtDateTime;
        }

        /// <summary>
        /// Initializes a new instance of the simple finite state machine history entry.
        /// </summary>
        /// <param name="entryNumber">The entry number.</param>
        /// <param name="stateName">The state name.</param>
        /// <param name="reasonName">The reason name.</param>
        /// <param name="comments">The comments.</param>
        /// <param name="clock">The system clock abstraction.</param>
        public StateMachineHistoryEntry(
            int entryNumber,
            TStateName stateName,
            TReasonName reasonName,
            string comments,
            IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            Sid = Guid.NewGuid();
            Comments = comments;

            EntryNumber = entryNumber;
            StateName = stateName;
            ReasonName = reasonName;

            EntryDate = clock.Now();
        }

        /// <summary>
        /// Initializes a new instance of the simple finite state machine history entry.
        /// </summary>
        /// <param name="entryNumber">The entry number.</param>
        /// <param name="stateName">The state name.</param>
        /// <param name="reasonName">The reason name.</param>
        /// <param name="comments">The comments.</param>
        public StateMachineHistoryEntry(
            int entryNumber,
            TStateName stateName,
            TReasonName reasonName,
            string comments)
            : this(entryNumber, stateName, reasonName, comments, new Clock())
        {
        }

        /// <summary>
        /// Initializes a new instance of the simple finite state machine history entry.
        /// </summary>
        /// <param name="entryNumber">The entry number.</param>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="reasonName">The reason code.</param>
        public StateMachineHistoryEntry(int entryNumber, TStateName stateName, TReasonName reasonName)
            : this(entryNumber, stateName, reasonName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the simple finite state machine history entry.
        /// </summary>
        /// <param name="stateName">The state name.</param>
        /// <param name="reasonName">The reason name.</param>
        /// <param name="comments">The comments.</param>
        public StateMachineHistoryEntry(TStateName stateName, TReasonName reasonName, string comments)
            : this(-1, stateName, reasonName, comments)
        {
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string Comments { get; }

        /// <summary>
        /// Gets the entry date.
        /// </summary>
        public DateTimeOffset EntryDate { get; internal set; }

        /// <summary>
        /// Gets or sets the entry number. Typically set by a state machine. -1 = new entry not set by a state machine yet.
        /// </summary>
        public int EntryNumber { get; set; }

        /// <summary>
        /// Gets or sets the reason name.
        /// </summary>
        public TReasonName ReasonName { get; }

        /// <summary>
        /// Gets or sets the surrogate id.
        /// </summary>
        /// <remarks>
        /// This field is usually for persistence-related concerns.
        /// </remarks>
        public Guid Sid { get; }

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        public TStateName StateName { get; }

        /// <summary>
        /// Compares two history entries.
        /// </summary>
        /// <param name="left">The first entry to compare.</param>
        /// <param name="right">The second entry to compare.</param>
        /// <returns><c>true</c> if the two instances are equal. Otherwise, <c>false</c>.</returns>
        public static bool operator ==(StateMachineHistoryEntry<TStateName, TReasonName> left, StateMachineHistoryEntry<TStateName, TReasonName> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two history entries.
        /// </summary>
        /// <param name="left">The first entry to compare.</param>
        /// <param name="right">The second entry to compare.</param>
        /// <returns><c>false</c> if the two instances are equal. Otherwise, <c>true</c>.</returns>
        public static bool operator !=(StateMachineHistoryEntry<TStateName, TReasonName> left, StateMachineHistoryEntry<TStateName, TReasonName> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Compares this instance with another.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns><c>true</c> if the two instances are equal. Otherwise, <c>false</c>.</returns>
        public bool Equals(StateMachineHistoryEntry<TStateName, TReasonName> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Comments == other.Comments &&
                   EqualityComparer<TReasonName>.Default.Equals(ReasonName, other.ReasonName) &&
                   EqualityComparer<TStateName>.Default.Equals(StateName, other.StateName);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((StateMachineHistoryEntry<TStateName, TReasonName>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Comments, ReasonName, StateName);
        }
    }
}
