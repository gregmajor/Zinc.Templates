using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using RedLine.Data.Outbox;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.OutboxTests
{
    public class DispatchEventsShould : FunctionalTestBase
    {
        public DispatchEventsShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        private IOutbox Outbox => GetRequiredService<IOutbox>();

        private IDbConnection Connection => GetRequiredService<IDbConnection>();

        [Fact]
        public async Task ReturnWithoutDispatching()
        {
            // Arrange
            static Task<int> Dispatch(OutboxRecord record) => throw new Exception();

            // Act
            var result = await Outbox.DispatchEvents("unusedDispatcherId", Dispatch).ConfigureAwait(false);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task DeleteRecordsAfterDispatching()
        {
            // Arrange
            var dispatchId = "dispatchId";
            static Task<int> Dispatch(OutboxRecord record) => Task.FromResult(1);
            await Connection.ExecuteAsync("insert into outbox.outbox (id, messages) values (@id, '[]'::jsonb)", new { id = Guid.NewGuid() }).ConfigureAwait(false);

            // Act
            var result = await Outbox.DispatchEvents(dispatchId, Dispatch).ConfigureAwait(false);

            // Assert
            var remainingRecords = Connection.QuerySingle<int>("select count(1) from outbox.outbox");
            remainingRecords.Should().Be(0);
            result.Should().Be(1);
        }
    }
}
