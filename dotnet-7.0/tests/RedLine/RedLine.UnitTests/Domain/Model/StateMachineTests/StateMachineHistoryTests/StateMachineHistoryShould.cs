using FluentAssertions;
using RedLine.Domain.Model.StateMachine;
using Xunit;

namespace RedLine.UnitTests.Domain.Model.StateMachineTests.StateMachineHistoryTests
{
    public class StateMachineHistoryShould
    {
        [Fact]
        public void MostRecentEntryShouldBeLastEntryAdded()
        {
            // Arrange
            var stateMachineHistory = new StateMachineHistory<int, int>();

            // Act
            stateMachineHistory.AddEntry(new StateMachineHistoryEntry<int, int>(0, 0, "A comment"));
            stateMachineHistory.AddEntry(new StateMachineHistoryEntry<int, int>(0, 0, "Another comment"));

            var lastEntry = new StateMachineHistoryEntry<int, int>(0, 0, "Yet another comment");
            stateMachineHistory.AddEntry(lastEntry);

            // Assert
            stateMachineHistory.MostRecentEntry.Should().Be(lastEntry);
        }

        [Fact]
        public void InitialEntryShouldBeFirstEntryAdded()
        {
            // Arrange
            var stateMachineHistory = new StateMachineHistory<int, int>();

            // Act
            var firstEntry = new StateMachineHistoryEntry<int, int>(0, 0, "A comment");
            stateMachineHistory.AddEntry(firstEntry);

            stateMachineHistory.AddEntry(new StateMachineHistoryEntry<int, int>(0, 0, "Another comment"));
            stateMachineHistory.AddEntry(new StateMachineHistoryEntry<int, int>(0, 0, "Yet another comment"));

            // Assert
            stateMachineHistory.InitialEntry.Should().Be(firstEntry);
        }

        [Fact]
        public void EnumerationShouldMatchEntries()
        {
            // Arrange
            var stateMachineHistory = new StateMachineHistory<int, int>();

            var firstEntry = new StateMachineHistoryEntry<int, int>(0, 0, "A comment");
            var secondEntry = new StateMachineHistoryEntry<int, int>(0, 0, "Another comment");
            var thirdEntry = new StateMachineHistoryEntry<int, int>(0, 0, "Yet another comment");

            var entries = new[]
            {
                firstEntry,
                secondEntry,
                thirdEntry,
            };

            var i = 0;

            // Act
            foreach (var entry in entries)
            {
                stateMachineHistory.AddEntry(entry);
            }

            // Assert
            foreach (var entry in stateMachineHistory.Entries)
            {
                entry.Should().Be(entries[i]);
                entry.EntryNumber.Should().Be(i + 1);

                i++;
            }
        }
    }
}
