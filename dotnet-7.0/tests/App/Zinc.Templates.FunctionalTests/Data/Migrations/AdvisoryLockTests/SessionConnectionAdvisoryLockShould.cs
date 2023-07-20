using System;
using Dapper;
using FluentAssertions;
using Npgsql;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Data.Migrations;

namespace Zinc.Templates.FunctionalTests.Data.Migrations.AdvisoryLockTests
{
    public class SessionConnectionAdvisoryLockShould : AdvisoryLockTestBase
    {
        public SessionConnectionAdvisoryLockShould(FunctionalTestFixture fixture, ITestOutputHelper outputHelper)
            : base(fixture, outputHelper)
        {
        }

        [Theory(Skip = "This just tests the test base, not the real code.")]
        [InlineData(13)]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void GetLocksCalculatesCorrectIdFromLong(long lockId)
        {
            using (var conn = OpenConnection())
            {
                conn.Execute($"SELECT pg_advisory_lock(CAST({lockId} AS BIGINT));");
                GetLocks().Should().Contain(lockId);
            }
        }

        [Theory(Skip = "This just tests the test base, not the real code.")]
        [InlineData(13)]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void GetLocksCalculatesCorrectIdFromInt(int lockId)
        {
            using (var conn = OpenConnection())
            {
                conn.Execute($"SELECT pg_advisory_lock(CAST({lockId} AS BIGINT));");
                GetLocks().Should().Contain(Convert.ToInt64(lockId));
            }
        }

        [Fact]
        public void CreateAdvisoryLockWhenInstantiatedWithConnection()
        {
            var lockId = GetRandomLockId();
            using (var conn = OpenConnection())
            using (var advisoryLock = new AdvisoryLock(conn, lockId))
            {
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
            using (var advisoryLock = new AdvisoryLock(conn, lockId))
            {
                var lockIds = GetLocks();
                lockIds.Should().Contain(lockId);
                foreach (var acquiredLockId in lockIds)
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                Output.WriteLine("Opening second connection.");
                using (var conn2 = OpenConnection())
                {
                    conn.ProcessID.Should().NotBe(conn2.ProcessID);
                    Assert.Throws<NpgsqlException>(() => new AdvisoryLock(conn2, lockId));
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
            using (var advisoryLock = new AdvisoryLock(conn, lockId))
            {
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
        public void ReleaseAdvisoryLockWhenConnectionClosed()
        {
            var lockId = GetRandomLockId();
            var conn = OpenConnection();
            using (var advisoryLock = new AdvisoryLock(conn, lockId))
            {
                foreach (var acquiredLockId in GetLocks())
                {
                    Output.WriteLine($"Acquired lock id: {acquiredLockId}");
                }

                Output.WriteLine("Closing connection without explicitly releasing lock.");
                conn.Close();

                // Instead of just returning to the connection pool, we need to force the physical connection to close,
                // just like in a program crash.
                NpgsqlConnection.ClearPool(conn);
                conn.Dispose();

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
