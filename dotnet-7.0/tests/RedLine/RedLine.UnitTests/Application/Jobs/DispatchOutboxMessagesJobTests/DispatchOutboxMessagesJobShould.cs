using System;
using System.Transactions;
using FluentAssertions;
using RedLine.Application.Jobs.Outbox;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.DispatchOutboxMessagesJobTests
{
    [Collection(nameof(UnitTestCollection))]
    public class DispatchOutboxMessagesJobShould
    {
        [Fact]
        public void SetDispatcherIdIfProvided()
        {
            // Arrange
            var dispatcherId = "123";
            var activity = new DispatchOutboxMessagesJob("TenantId", Guid.NewGuid(), dispatcherId);

            // Assert
            activity.DispatcherId.Should().Be(dispatcherId);
        }

        [Fact]
        public void SetDispatcherIdToDefaultIfNotSpecified()
        {
            // Arrange
            var activity = new DispatchOutboxMessagesJob("TenantId", Guid.NewGuid());

            // Assert
            activity.DispatcherId.Should().Be(DispatchOutboxMessagesJob.DefaultDispatcherId);
        }

        [Fact]
        public void SetTransactionIsolationToSnapshot()
        {
            // Arrange
            var activity = new DispatchOutboxMessagesJob("TenantId", Guid.NewGuid());

            // Assert
            activity.TransactionIsolation.Should().Be(IsolationLevel.Snapshot);
        }
    }
}
