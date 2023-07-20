using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using RedLine.Application.Jobs.Outbox;
using RedLine.Data.Outbox;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.DispatchOutboxMessagesJobTests
{
    [Collection(nameof(UnitTestCollection))]
    public class DispatchOutboxMessagesJobHandlerShould
    {
        [Fact]
        public async Task PublishOutboxMessagesToTheBus()
        {
            // Arrange
            var busMock = new Mock<IMessageSession>();

            busMock.Setup(x => x.Publish(
                It.IsAny<object>(),
                It.IsAny<PublishOptions>()))
                .Verifiable();

            var outboxMock = new Mock<IOutbox>();

            outboxMock.Setup(x => x.DispatchEvents(
                DispatchOutboxMessagesJob.DefaultDispatcherId,
                It.IsAny<Func<OutboxRecord, Task<int>>>()))
                .Verifiable();

            var expectedMessageCount = OutboxRecord().Messages.Count();

            var activity = new DispatchOutboxMessagesJob("Tenant", Guid.NewGuid());

            // Act
            var handler = new DispatchOutboxMessagesJobHandler(outboxMock.Object, busMock.Object);

            // This line tests to make sure that Outbox.DispatchEvents() is called.
            await handler.Handle(activity, default).ConfigureAwait(false);

            // This line tests that the Dispatch() function passed into Outbox.DispatchEvents() works correctly.
            var result = await handler.Dispatch(OutboxRecord()).ConfigureAwait(false);

            // Assert
            busMock.Verify(
                x => x.Publish(
                    It.IsAny<object>(),
                    It.IsAny<PublishOptions>()),
                Times.Exactly(expectedMessageCount));

            outboxMock.Verify();

            result.Should().Be(expectedMessageCount);
        }

        [Fact]
        public async Task SendOutboxMessageToDestinationOnTheBus()
        {
            // Arrange
            var destination = "destination";
            var busMock = new Mock<IMessageSession>();

            busMock.Setup(x => x.Send(
                It.IsAny<object>(),
                It.IsAny<SendOptions>()))
                .Verifiable();

            var outboxMock = new Mock<IOutbox>();

            outboxMock.Setup(x => x.DispatchEvents(
                DispatchOutboxMessagesJob.DefaultDispatcherId,
                It.IsAny<Func<OutboxRecord, Task<int>>>()))
                .Verifiable();

            var expectedMessageCount = OutboxRecord().Messages.Count();

            var activity = new DispatchOutboxMessagesJob("Tenant", Guid.NewGuid());

            // Act
            var handler = new DispatchOutboxMessagesJobHandler(outboxMock.Object, busMock.Object);

            // This line tests to make sure that Outbox.DispatchEvents() is called.
            await handler.Handle(activity, default).ConfigureAwait(false);

            // This line tests that the Dispatch() function passed into Outbox.DispatchEvents() works correctly.
            var result = await handler.Dispatch(OutboxRecord(destination)).ConfigureAwait(false);

            // Assert
            busMock.Verify(
                x => x.Send(
                    It.IsAny<object>(),
                    It.Is<SendOptions>(o => o.GetDestination() == destination)),
                Times.Exactly(expectedMessageCount));

            outboxMock.Verify();

            result.Should().Be(expectedMessageCount);
        }

        private OutboxRecord OutboxRecord(string destination = null)
        {
            return new OutboxRecord
            {
                DispatcherId = DispatchOutboxMessagesJob.DefaultDispatcherId,
                Id = Guid.NewGuid(),
                Messages = new[]
                {
                    new OutboxMessage
                    {
                        Destination = destination,
                        MessageId = Guid.NewGuid().ToString(),
                        MessageBody = new DomainEvent(1),
                        MessageHeaders = new Dictionary<string, string>
                        {
                            { "key1", "value1" },
                            { "key2", "value2" },
                        },
                    },
                    new OutboxMessage
                    {
                        Destination = destination,
                        MessageId = Guid.NewGuid().ToString(),
                        MessageBody = new DomainEvent(2),
                        MessageHeaders = new Dictionary<string, string>
                        {
                            { "key1", "value1" },
                            { "key2", "value2" },
                        },
                    },
                },
            };
        }

        internal class DomainEvent : RedLine.Domain.Events.IDomainEvent
        {
            public DomainEvent(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }
    }
}
