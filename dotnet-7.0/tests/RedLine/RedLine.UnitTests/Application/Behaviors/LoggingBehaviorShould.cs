using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RedLine.Application;
using RedLine.Application.Behaviors;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Application.Behaviors
{
    [Collection(nameof(UnitTestCollection))]
    public class LoggingBehaviorShould
    {
        private readonly TenantId tenantId = new();
        private readonly CorrelationId correlationId = new();
        private readonly Mock<IActivity> requestMock = new();
        private readonly Mock<IDisposable> disposableMock = new();

        [Fact]
        public async Task CreateScopeWithTenantIdAndCorrelationId()
        {
            // Arrange
            Mock<ILogger<LoggingBehavior<object, MyResponse>>> loggerMock = new();
            loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>())).Returns(disposableMock.Object);
            LoggingBehavior<object, MyResponse> behavior = new(loggerMock.Object);
            requestMock.Setup(x => x.CorrelationId).Returns(correlationId.Value);
            requestMock.Setup(x => x.TenantId).Returns(tenantId.Value);

            // Act
            await behavior.Handle(requestMock.Object, CancellationToken.None, Next).ConfigureAwait(false);

            // Assert
            Func<Dictionary<string, object>, bool> predicate = d => object.Equals(d[nameof(CorrelationId)], correlationId.Value) && object.Equals(d[nameof(TenantId)], tenantId.Value);
            loggerMock.Verify(x => x.BeginScope(It.Is<Dictionary<string, object>>(d => predicate(d))));
        }

        [Fact]
        public async Task LogRequestAndResponse()
        {
            // Arrange
            Mock<ILogger<LoggingBehavior<object, MyResponse>>> loggerMock = new();
            loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>())).Returns(disposableMock.Object);
            LoggingBehavior<object, MyResponse> behavior = new(loggerMock.Object);

            var requestMessage = TypeNameHelper.GetTypeDisplayName(typeof(object), false);
            var responseMessage = TypeNameHelper.GetTypeDisplayName(typeof(MyResponse), false);

            // Act
            await behavior.Handle(requestMock.Object, CancellationToken.None, Next).ConfigureAwait(false);

            // Assert
            Func<string, bool> valuesPredicate = m => m.Contains(requestMessage) && m.Contains(responseMessage);
            loggerMock.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsSubtype<object>>((m, t) => valuesPredicate(m.ToString())), null, It.IsAny<Func<It.IsSubtype<object>, Exception, string>>()));
        }

        [Fact]
        public async Task LogValueResponsesUsingToString()
        {
            // Arrange
            Mock<ILogger<LoggingBehavior<object, int>>> loggerMock = new();
            loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>())).Returns(disposableMock.Object);
            LoggingBehavior<object, int> behavior = new(loggerMock.Object);

            var requestMessage = TypeNameHelper.GetTypeDisplayName(typeof(object), false);

            // Act
            var response = await behavior.Handle(requestMock.Object, CancellationToken.None, NextInt).ConfigureAwait(false);

            // Assert
            Func<string, bool> valuesPredicate = m => m.Contains(requestMessage) && m.Contains(response.ToString());
            loggerMock.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsSubtype<object>>((m, t) => valuesPredicate(m.ToString())), null, It.IsAny<Func<It.IsSubtype<object>, Exception, string>>()));
        }

        [Fact]
        public async Task NotLogDefaultValueForResponse()
        {
            // Arrange
            Mock<ILogger<LoggingBehavior<object, int>>> loggerMock = new();
            loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>())).Returns(disposableMock.Object);
            LoggingBehavior<object, int> behavior = new(loggerMock.Object);

            var requestMessage = TypeNameHelper.GetTypeDisplayName(typeof(object), false);

            // Act
            var response = await behavior.Handle(requestMock.Object, CancellationToken.None, NextIntDefault).ConfigureAwait(false);

            // Assert
            // log message should not contain the responseMessage, because its value is default(int).
            Func<string, bool> valuesPredicate = m => m.Contains(requestMessage) && !m.Contains(response.ToString());
            loggerMock.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsSubtype<object>>((m, t) => valuesPredicate(m.ToString())), null, It.IsAny<Func<It.IsSubtype<object>, Exception, string>>()));
        }

        [Fact]
        public async Task LogAggregateException()
        {
            // Arrange
            Mock<ILogger<LoggingBehavior<object, int>>> loggerMock = new();
            loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>())).Returns(disposableMock.Object);
            LoggingBehavior<object, int> behavior = new(loggerMock.Object);
            var requestMessage = TypeNameHelper.GetTypeDisplayName(typeof(object), false);

            // Act
            Func<Task<int>> action = () => behavior.Handle(requestMock.Object, CancellationToken.None, NextAggregateException);

            // Assert
            await action.Should().ThrowAsync<AggregateException>().ConfigureAwait(false);
            Func<string, bool> valuesPredicate = m => m.Contains(requestMessage) && m.Contains("500") && m.Contains(nameof(NextAggregateException));
            loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsSubtype<object>>((m, t) => valuesPredicate(m.ToString())), It.IsAny<Exception>(), It.IsAny<Func<It.IsSubtype<object>, Exception, string>>()));
        }

        [Fact]
        public async Task LogAnyException()
        {
            // Arrange
            Mock<ILogger<LoggingBehavior<object, int>>> loggerMock = new();
            loggerMock.Setup(x => x.BeginScope(It.IsAny<Dictionary<string, object>>())).Returns(disposableMock.Object);
            LoggingBehavior<object, int> behavior = new(loggerMock.Object);
            var requestMessage = TypeNameHelper.GetTypeDisplayName(typeof(object), false);

            // Act
            Func<Task<int>> action = () => behavior.Handle(requestMock.Object, CancellationToken.None, NextAnyException);

            // Assert
            await action.Should().ThrowAsync<Exception>().ConfigureAwait(false);
            Func<string, bool> valuesPredicate = m => m.Contains(requestMessage) && m.Contains("500") && m.Contains(nameof(NextAnyException));
            loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsSubtype<object>>((m, t) => valuesPredicate(m.ToString())), It.IsAny<Exception>(), It.IsAny<Func<It.IsSubtype<object>, Exception, string>>()));
        }

        private Task<MyResponse> Next()
        {
            return Task.FromResult(new MyResponse());
        }

        private Task<int> NextIntDefault()
        {
            return Task.FromResult(default(int));
        }

        private Task<int> NextInt()
        {
            return Task.FromResult(7);
        }

        private async Task<int> NextAggregateException()
        {
            await Task.CompletedTask.ConfigureAwait(false);
            throw new AggregateException(new Exception(nameof(NextAggregateException)));
        }

        private async Task<int> NextAnyException()
        {
            await Task.CompletedTask.ConfigureAwait(false);
            throw new Exception(nameof(NextAnyException));
        }

        internal class MyResponse
        {
            public bool Initiated { get; set; } = true;
        }
    }
}
