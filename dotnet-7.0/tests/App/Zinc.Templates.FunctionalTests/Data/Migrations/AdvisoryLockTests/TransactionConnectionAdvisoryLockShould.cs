using FluentAssertions;
using Npgsql;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Data.Migrations;

namespace Zinc.Templates.FunctionalTests.Data.Migrations.AdvisoryLockTests
{
    public class TransactionConnectionAdvisoryLockShould : AdvisoryLockTestBase
    {
        public TransactionConnectionAdvisoryLockShould(FunctionalTestFixture fixture, ITestOutputHelper outputHelper)
            : base(fixture, outputHelper)
        {
        }

        [Fact]
        public void CreateAdvisoryLockWithTransaction()
        {
            var lockId = GetRandomLockId();
            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                AdvisoryLock.AcquireLock(tx, lockId);
                var lockIds = GetLocks();
                lockIds.Should().Contain(lockId);
                foreach (var acquiredLockId in lockIds)
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                Output.WriteLine($"Releasing lock id {lockId}");
            }

            foreach (var acquiredLockId in GetLocks())
            {
                Output.WriteLine($"Acquired lock id: {acquiredLockId}");
            }
        }

        [Fact(Skip = "CommandTimeout is 30 seconds, so this test slow.")]
        public void BlockWhenLockAlreadyExists()
        {
            var lockId = GetRandomLockId();
            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                AdvisoryLock.AcquireLock(tx, lockId);
                var lockIds = GetLocks();
                lockIds.Should().Contain(lockId);
                foreach (var acquiredLockId in lockIds)
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                Output.WriteLine("Opening second connection and transaction.");
                using (var conn2 = OpenConnection())
                using (var tx2 = conn2.BeginTransaction())
                {
                    conn.ProcessID.Should().NotBe(conn2.ProcessID);
                    Assert.Throws<NpgsqlException>(() => AdvisoryLock.AcquireLock(tx2, lockId));
                }

                Output.WriteLine($"Releasing lock id {lockId}");
            }

            foreach (var acquiredLockId in GetLocks())
            {
                Output.WriteLine($"Acquired lock id: {acquiredLockId}");
            }
        }

        [Fact]
        public void ReleaseAdvisoryLockWhenDisposed()
        {
            var lockId = GetRandomLockId();
            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                AdvisoryLock.AcquireLock(tx, lockId);
                foreach (var acquiredLockId in GetLocks())
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                Output.WriteLine($"Releasing lock {lockId}");
            }

            var lockIds = GetLocks();
            lockIds.Should().NotContain(lockId);
            foreach (var acquiredLockId in lockIds)
            {
                Output.WriteLine($"Acquired lock id: {acquiredLockId}");
            }
        }

        [Fact]
        public void ReleaseAdvisoryLockWhenTransactionCommitted()
        {
            var lockId = GetRandomLockId();
            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                AdvisoryLock.AcquireLock(tx, lockId);
                foreach (var acquiredLockId in GetLocks())
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                tx.Commit();

                var acquiredLockIds = GetLocks();
                foreach (var acquiredLockId in acquiredLockIds)
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                acquiredLockIds.Should().NotContain(lockId);

                Output.WriteLine($"Releasing lock {lockId}");
            }
        }

        [Fact]
        public void ReleaseAdvisoryLockWhenTransactionRolledBack()
        {
            var lockId = GetRandomLockId();
            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
            {
                AdvisoryLock.AcquireLock(tx, lockId);
                foreach (var acquiredLockId in GetLocks())
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                tx.Rollback();

                var acquiredLockIds = GetLocks();
                foreach (var acquiredLockId in acquiredLockIds)
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                acquiredLockIds.Should().NotContain(lockId);

                Output.WriteLine($"Releasing lock {lockId}");
            }
        }
    }
}
