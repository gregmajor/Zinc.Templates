using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using RedLine.Data.Outbox;
using RedLine.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.OutboxTests;

public class SaveCommandShould : FunctionalTestBase
{
    public SaveCommandShould(FunctionalTestFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    private IOutbox Outbox => GetRequiredService<IOutbox>();

    [Fact]
    public async Task SaveCommand()
    {
        // Arrange
        var command = new Command(Guid.NewGuid());
        var db = GetRequiredService<IDbConnection>();

        // Act
        await Outbox.SaveCommandToThisApplication(command);

        // Assert
        var record = await db.QueryFirstAsync<OutboxRecord>("select * from outbox.outbox").ConfigureAwait(false);
        record.Messages.Count().Should().Be(1);
        var outboxMessage = record.Messages.First();
        outboxMessage.MessageBody.Should().BeOfType<Command>();
        ((Command)outboxMessage.MessageBody).Message.Should().Be(command.Message);
        outboxMessage.Destination.Should().Be(ApplicationContext.ApplicationName);
    }

    [Fact]
    public async Task SaveCommandWithDestination()
    {
        // Arrange
        var destination = "Crazy Place";
        var command = new Command(Guid.NewGuid());
        var db = GetRequiredService<IDbConnection>();

        // Act
        await Outbox.SaveCommand(command, destination);

        // Assert
        var record = await db.QueryFirstAsync<OutboxRecord>("select * from outbox.outbox").ConfigureAwait(false);
        record.Messages.Count().Should().Be(1);
        var outboxMessage = record.Messages.First();
        outboxMessage.MessageBody.Should().BeOfType<Command>();
        ((Command)outboxMessage.MessageBody).Message.Should().Be(command.Message);
        outboxMessage.Destination.Should().Be(destination);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public async Task ThrowForMissingDestination(string destination)
    {
        // Arrange
        var command = new Command(Guid.NewGuid());

        // Act
        Func<Task> act = () => Outbox.SaveCommand(command, destination);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName(nameof(destination));
    }

    [Fact]
    public async Task ThrowForMissingCommand()
    {
        // Act
        Func<Task> act = () => Outbox.SaveCommand(null, "destination");

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("command");
    }

    private sealed record Command(Guid Message);
}
