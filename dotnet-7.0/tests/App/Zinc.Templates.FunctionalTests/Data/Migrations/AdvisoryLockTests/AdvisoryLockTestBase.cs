using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RedLine.Data;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.Migrations.AdvisoryLockTests
{
    [Collection(nameof(FunctionalTestCollection))]
    public abstract class AdvisoryLockTestBase : FunctionalTestBase
    {
        private readonly RandomNumberGenerator random;

        protected AdvisoryLockTestBase(FunctionalTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.random = RandomNumberGenerator.Create();
        }

        protected PostgresConnectionString ConnectionString => Fixture.ServiceProvider.GetRequiredService<PostgresConnectionString>();

        protected NpgsqlConnection OpenConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString.Value);
            conn.Open();
            return conn;
        }

        protected long GetRandomLockId()
        {
            var data = new byte[8];

            random.GetBytes(data);
            var value1 = BitConverter.ToInt64(data);
            random.GetBytes(data);
            var value2 = BitConverter.ToInt64(data);

            return GetLockId(value1, value2);
        }

        protected long[] GetLocks()
        {
            // https://www.postgresql.org/docs/9.3/view-pg-locks.html
            using (var conn = OpenConnection())
            using (var da = new NpgsqlDataAdapter("SELECT * FROM pg_locks", conn))
            {
                var ds = new DataSet();
                da.Fill(ds);
                var locks = ds.Tables[0].Rows
                    .Cast<DataRow>()
                    .Select(r => new
                    {
                        LockType = r.IsNull("locktype") ? (string)null : (string)r["locktype"],
                        ClassId = r.IsNull("classid") ? (uint?)null : (uint)r["classid"],
                        ObjectId = r.IsNull("objid") ? (uint?)null : (uint)r["objid"],
                    })
                    .ToArray();

                var advisoryLockIds = locks
                    .Where(x => string.Equals(x.LockType, "advisory", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => GetLockId(x.ClassId ?? 0, x.ObjectId ?? 0))
                    .ToArray();
                return advisoryLockIds;
            }
        }

        private static long GetLockId(long classId, long objectId)
        {
            // https://www.postgresql.org/docs/9.3/view-pg-locks.html
            var value = (classId << 32) | objectId;
            return value;
        }

        private static long GetLockId(uint classId, uint objectId)
        {
            return GetLockId((long)classId, (long)objectId);
        }

        private static long GetLockId(int classId, int objectId)
        {
            return GetLockId((long)classId, (long)objectId);
        }
    }
}
