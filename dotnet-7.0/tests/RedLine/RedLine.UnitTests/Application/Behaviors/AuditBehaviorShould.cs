using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Krypton.Audit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using RedLine.A3;
using RedLine.Application;
using RedLine.Application.Behaviors;
using RedLine.Application.Jobs;
using RedLine.Data.Outbox;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Application.Behaviors
{
    [Collection(nameof(UnitTestCollection))]
    public class AuditBehaviorShould
    {
        [Fact]
        public async Task PublishEventWhenAuditable()
        {
            // Arrange
            Mock<IMessageSession> bus = new();
            AuditBehavior<Request, string> behavior = new(Context(), bus.Object, null, Outbox().Object);
            Request request = new();
            static Task<string> Next()
            {
                return Task.FromResult(string.Empty);
            }

            // Act
            await behavior.Handle(request, default, Next).ConfigureAwait(false);

            // Assert
            bus.Verify(b => b.Publish(It.IsAny<ActivityAudited>(), It.IsAny<PublishOptions>()), Times.Once);
        }

        [Fact]
        public async Task DontPublishWhenNoWorkPerformedJob()
        {
            // Arrange
            Mock<IMessageSession> bus = new();
            AuditBehavior<Request, JobResult> behavior = new(Context(), bus.Object, null, Outbox().Object);
            Request request = new();
            static Task<JobResult> Next()
            {
                return Task.FromResult(JobResult.NoWorkPerformed);
            }

            // Act
            await behavior.Handle(request, default, Next).ConfigureAwait(false);

            // Assert
            bus.Verify(b => b.Publish(It.IsAny<ActivityAudited>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Fact]
        public async Task PublishWhenOperationSucceededJob()
        {
            // Arrange
            Mock<IMessageSession> bus = new();
            AuditBehavior<Request, JobResult> behavior = new(Context(), bus.Object, null, Outbox().Object);
            Request request = new();
            static Task<JobResult> Next()
            {
                return Task.FromResult(JobResult.OperationSucceeded);
            }

            // Act
            await behavior.Handle(request, default, Next).ConfigureAwait(false);

            // Assert
            bus.Verify(b => b.Publish(It.IsAny<ActivityAudited>(), It.IsAny<PublishOptions>()), Times.Once);
        }

        [Fact]
        public async Task PublishWhenJobThrows()
        {
            // Arrange
            Mock<IMessageSession> bus = new();
            AuditBehavior<Request, JobResult> behavior = new(Context(), bus.Object, null, Outbox().Object);
            Request request = new();
            static Task<JobResult> Next()
            {
                throw new Exception();
            }

            // Act
            await Assert.ThrowsAsync<Exception>(async () => await behavior.Handle(request, default, Next).ConfigureAwait(false))
                .ConfigureAwait(false);

            // Assert
            bus.Verify(b => b.Publish(It.IsAny<ActivityAudited>(), It.IsAny<PublishOptions>()), Times.Once);
        }

        [Fact]
        public async Task CallNextWhenNotAuditable()
        {
            // Arrange
            AuditBehavior<string, string> behavior = new(null, null, null, null);
            var called = false;
            Task<string> Next()
            {
                called = true;
                return Task.FromResult(string.Empty);
            }

            // Act
            await behavior.Handle(string.Empty, default, Next).ConfigureAwait(false);

            // Assert
            called.Should().BeTrue();
        }

        [Fact]
        public async Task SaveToOutboxWhenRabbitIsDown()
        {
            // Arrange
            Mock<IMessageSession> bus = new();
            var outbox = Outbox();
            var logger = Logger();

            bus
                .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .ThrowsAsync(new RabbitMQ.Client.Exceptions.ConnectFailureException("Rabbit Down", new Exception()));

            AuditBehavior<Request, string> behavior = new(
                Context(),
                bus.Object,
                logger.Object,
                outbox.Object);

            Request request = new();

            static Task<string> Next()
            {
                return Task.FromResult(string.Empty);
            }

            // Act
            await behavior.Handle(request, default, Next).ConfigureAwait(false);

            // Assert
            bus.Verify(b => b.Publish(It.IsAny<ActivityAudited>(), It.IsAny<PublishOptions>()), Times.Once);
            logger.Verify();
            outbox.Verify(o => o.SaveMessages(It.IsAny<IEnumerable<OutboxMessage>>()), Times.Once);
        }

        private static IActivityContext Context()
        {
            Mock<IDbConnection> connection = new();
            Mock<IAccessToken> accessToken = new();

            return new ActivityContext(
                new TenantId(nameof(UnitTestCollection)),
                new CorrelationId(),
                new ETag(),
                connection.Object,
                accessToken.Object,
                new ClientAddress(),
                new ServiceCollection().BuildServiceProvider());
        }

        private static Mock<IOutbox> Outbox()
        {
            Mock<IOutbox> mock = new();

            mock
                .Setup(x => x.SaveMessages(It.IsAny<IEnumerable<OutboxMessage>>()))
                .ReturnsAsync(1);

            return mock;
        }

        private static Mock<ILogger<AuditBehavior<Request, string>>> Logger()
        {
            Mock<ILogger<AuditBehavior<Request, string>>> mock = new();

            mock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>()));
            mock.Setup(x => x.Log(
                It.Is<LogLevel>(match => match == LogLevel.Critical),
                It.IsAny<EventId>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<Dictionary<string, object>, Exception, string>>()));

            return mock;
        }

        internal class Request : IAmAuditable
        {
        }
    }
}
