using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.Domain.Model.StateMachine
{
    /// <summary>
    /// The finite state machine history.
    /// </summary>
    /// <typeparam name="TStateName">The type of the state name (usually an enum).</typeparam>
    /// <typeparam name="TReasonName">The type of the reason name (usually an enum).</typeparam>
    public class StateMachineHistory<TStateName, TReasonName>
    {
        private readonly IClock clock;

        private readonly ConcurrentDictionary<int, StateMachineHistoryEntry<TStateName, TReasonName>> entries = new();

        /// <summary>
        /// Creates a new instance of the finite state machine history.
        /// </summary>
        /// <param name="clock">The system clock abstraction.</param>
        public StateMachineHistory(IClock clock)
        {
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <summary>
        /// Creates a new instance of the finite state machine history.
        /// </summary>
        public StateMachineHistory()
            : this(new Clock())
        {
        }

        /// <summary>
        /// Gets the entries sorted by entry number.
        /// </summary>
        public virtual IEnumerable<StateMachineHistoryEntry<TStateName, TReasonName>> Entries
        {
            get
            {
                if (!entries.Any())
                {
                    yield break;
                }

                for (var i = 1; i <= entries.Count; i++)
                {
                    if (entries.TryGetValue(i, out var entry))
                    {
                        yield return entry;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the initial entry.
        /// </summary>
        /// <value>
        /// The initial entry.
        /// </value>
        public virtual StateMachineHistoryEntry<TStateName, TReasonName> InitialEntry
        {
            get
            {
                if (entries.TryGetValue(1, out var entry))
                {
                    return entry;
                }

                return default;
            }
        }

        /// <summary>
        /// Gets the most recent entry.
        /// </summary>
        public virtual StateMachineHistoryEntry<TStateName, TReasonName> MostRecentEntry
        {
            get
            {
                if (entries.TryGetValue(entries.Count, out var entry))
                {
                    return entry;
                }

                return default;
            }
        }

        /// <summary>
        /// Adds an entry to the history.
        /// </summary>
        /// <param name="historyEntry">The history entry.</param>
        public virtual void AddEntry(StateMachineHistoryEntry<TStateName, TReasonName> historyEntry)
        {
            while (true)
            {
                historyEntry.EntryNumber = entries.Count + 1;

                if (entries.TryAdd(historyEntry.EntryNumber, historyEntry))
                {
                    if (historyEntry.EntryDate == default)
                    {
                        historyEntry.EntryDate = clock.Now();
                    }

                    return;
                }
            }
        }
    }
}
