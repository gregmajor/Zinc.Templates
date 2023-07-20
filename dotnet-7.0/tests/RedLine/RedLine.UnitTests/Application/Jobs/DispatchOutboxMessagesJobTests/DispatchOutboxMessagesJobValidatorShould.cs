using System;
using System.Linq;
using System.Transactions;
using FluentAssertions;
using RedLine.Application.Jobs.Outbox;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.DispatchOutboxMessagesJobTests
{
    [Collection(nameof(UnitTestCollection))]
    public class DispatchOutboxMessagesJobValidatorShould
    {
        [Theory]
        [InlineData(IsolationLevel.Snapshot)]
        [InlineData(IsolationLevel.Serializable)]
        [InlineData(IsolationLevel.Chaos)]
        [InlineData(IsolationLevel.ReadCommitted)]
        [InlineData(IsolationLevel.ReadUncommitted)]
        [InlineData(IsolationLevel.RepeatableRead)]
        [InlineData(IsolationLevel.Unspecified)]
        public void OnlyAllowSnapshotOrSerializableIsolationLevels(IsolationLevel isolationLevel)
        {
            // Arrange
            var activity = new DispatchOutboxMessagesJob("Tenant", Guid.NewGuid())
            {
                TransactionIsolation = isolationLevel,
            };

            var validator = new DispatchOutboxMessagesJobValidator();

            // Act
            var result = validator.Validate(activity);

            // Assert
            if (isolationLevel.Equals(IsolationLevel.Snapshot))
            {
                result.IsValid.Should().BeTrue();
            }
            else if (isolationLevel.Equals(IsolationLevel.Serializable))
            {
                result.IsValid.Should().BeTrue();
            }
            else
            {
                result.IsValid.Should().BeFalse();
            }
        }

        [Fact]
        public void NotAllowEmptyCorrelationId()
        {
            // Arrange
            var activity = new DispatchOutboxMessagesJob("Tenant", Guid.Empty);
            var validator = new DispatchOutboxMessagesJobValidator();

            // Act
            var result = validator.Validate(activity);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.First().PropertyName.Should().Be(nameof(DispatchOutboxMessagesJob.CorrelationId));
        }

        [Fact]
        public void NotAllowEmptyDispatcherId()
        {
            // Arrange
            var activity = new DispatchOutboxMessagesJob("Tenant", Guid.NewGuid(), string.Empty);
            var validator = new DispatchOutboxMessagesJobValidator();

            // Act
            var result = validator.Validate(activity);

            // Assert
            result.Errors.First().PropertyName.Should().Be(nameof(DispatchOutboxMessagesJob.DispatcherId));
        }

        [Fact]
        public void NotAllowNullOrEmptyTenantId()
        {
            // Arrange
            var activity1 = new DispatchOutboxMessagesJob(null, Guid.NewGuid());
            var activity2 = new DispatchOutboxMessagesJob(string.Empty, Guid.NewGuid());
            var validator = new DispatchOutboxMessagesJobValidator();

            // Act
            var result1 = validator.Validate(activity1);
            var result2 = validator.Validate(activity2);

            // Assert
            result1.IsValid.Should().BeFalse();
            result1.Errors.First().PropertyName.Should().Be(nameof(DispatchOutboxMessagesJob.TenantId));

            result2.IsValid.Should().BeFalse();
            result2.Errors.First().PropertyName.Should().Be(nameof(DispatchOutboxMessagesJob.TenantId));
        }
    }
}
