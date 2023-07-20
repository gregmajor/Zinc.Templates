using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Moq;
using RedLine.Data.Outbox;
using RedLine.Domain.Events;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.OutboxTests
{
    public class SaveEventsShould : FunctionalTestBase
    {
        public SaveEventsShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        private IOutbox Outbox => GetRequiredService<IOutbox>();

        [Fact]
        public async Task SaveEventsFromEventSource()
        {
            // Arrange
            var event1 = new DomainEvent();
            var event2 = new DomainEvent();
            var events = new List<IDomainEvent> { event1, event2 };
            var mockEventSource = new Mock<IAggregateRoot>();
            mockEventSource.SetupGet(x => x.Events).Returns(events);
            var db = GetRequiredService<IDbConnection>();

            // Act
            await Outbox.SaveEvents(mockEventSource.Object).ConfigureAwait(false);

            // Assert
            var record = await db.QueryFirstAsync<OutboxRecord>("select * from outbox.outbox").ConfigureAwait(false);
            record.Messages.Count().Should().Be(2);
            events.Should().BeEmpty();
            var first = record.Messages.First();
            var last = record.Messages.Last();
            first.MessageBody.Should().BeOfType<DomainEvent>();
            last.MessageBody.Should().BeOfType<DomainEvent>();
            ((DomainEvent)first.MessageBody).Id.Should().Be(event1.Id);
            ((DomainEvent)last.MessageBody).Id.Should().Be(event2.Id);
        }

        private class DomainEvent : IDomainEvent
        {
            public Guid Id { get; set; } = Guid.NewGuid();
        }
    }
}
