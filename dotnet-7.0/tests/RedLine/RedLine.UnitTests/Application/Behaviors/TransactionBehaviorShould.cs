using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using FluentAssertions;
using Moq;
using RedLine.Application;
using RedLine.Application.Behaviors;
using Xunit;

namespace RedLine.UnitTests.Application.Behaviors
{
    [Collection(nameof(UnitTestCollection))]
    public class TransactionBehaviorShould
    {
        private readonly Mock<IDbConnection> dbMock = new();
        private readonly object objectResponse = new object();
        private readonly Mock<IAmTransactional> transactionRequestMock = new();

        [Fact]
        public async Task CallNextWhenRequestIsNotTransactional()
        {
            // Arrange
            var behavior = new TransactionBehavior<object, object>(dbMock.Object);
            var request = new object();

            // Act
            var actual = await behavior.Handle(request, CancellationToken.None, NextObject).ConfigureAwait(false);

            // Assert
            actual.Should().Be(objectResponse);
        }

        [Fact]
        public async Task CallsNextInTransactionScope()
        {
            // Arrange
            var behavior = new TransactionBehavior<IAmTransactional, object>(dbMock.Object);
            var isolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            transactionRequestMock.Setup(x => x.TransactionTimeout).Returns(TimeSpan.FromMinutes(1));
            transactionRequestMock.Setup(x => x.TransactionIsolation).Returns(isolationLevel);

            Task<object> NextWithTransaction()
            {
                Transaction.Current.Should().NotBeNull();
                Transaction.Current.IsolationLevel.Should().Be(isolationLevel);
                return Task.FromResult(objectResponse);
            }

            // Act
            var actual = await behavior.Handle(transactionRequestMock.Object, CancellationToken.None, NextWithTransaction).ConfigureAwait(false);

            // Assert
            actual.Should().Be(objectResponse);
        }

        private Task<object> NextObject() => Task.FromResult(objectResponse);
    }
}
