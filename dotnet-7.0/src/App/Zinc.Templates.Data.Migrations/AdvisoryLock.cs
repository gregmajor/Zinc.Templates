using System;
using System.Data;
using Dapper;
using Npgsql;
using RedLine.Data;

namespace Zinc.Templates.Data.Migrations
{
    /// <summary>
    /// Creates an advisory lock.
    /// </summary>
    public class AdvisoryLock : IDisposable
    {
        private readonly IDisposable advisoryLock;

        /// <summary>
        /// Creates an advisory lock with lifecycle tied to the connection.
        /// </summary>
        /// <param name="connection">An open database connection.</param>
        /// <param name="lockId">The id of the lock.</param>
        public AdvisoryLock(IDbConnection connection, long lockId)
        {
            advisoryLock = new SessionAdvisoryLock(connection, lockId);
        }

        /// <summary>
        /// Creates an advisory lock with lifecycle tied to the connection.
        /// </summary>
        /// <param name="connection">An open database connection.</param>
        /// <param name="lockId">The id of the lock.</param>
        public AdvisoryLock(IDbConnection connection, int lockId)
            : this(connection, (long)lockId)
        {
        }

        /// <summary>
        /// Creates an advisory lock in the database.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="lockId">The id of the lock.</param>
        public AdvisoryLock(string connectionString, long lockId)
        {
            advisoryLock = new ManagedConnectionAdvisoryLock(connectionString, lockId);
        }

        /// <summary>
        /// Creates an advisory lock in the database.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="lockId">The id of the lock.</param>
        public AdvisoryLock(string connectionString, int lockId)
            : this(connectionString, (long)lockId)
        {
        }

        /// <summary>
        /// Create an advisory lock in the database.
        /// </summary>
        /// <param name="connectionString">Connection string of type <see cref="PostgresConnectionString" />.</param>
        /// <param name="lockId">The id of the lock.</param>
        public AdvisoryLock(PostgresConnectionString connectionString, long lockId)
        {
            advisoryLock = new ManagedConnectionAdvisoryLock(connectionString.Value, lockId);
        }

        /// <summary>
        /// Create an advisory lock in the database.
        /// </summary>
        /// <param name="connectionString">Connection string of type <see cref="PostgresConnectionString" />.</param>
        /// <param name="lockId">The id of the lock.</param>
        public AdvisoryLock(PostgresConnectionString connectionString, int lockId)
            : this(connectionString.Value, (long)lockId)
        {
        }

        /// <summary>
        /// Creates an advisory lock tied to the lifecycle of the transaction.
        /// </summary>
        /// <param name="transaction">The transaction controlling this lock.</param>
        /// <param name="lockId">The id of the lock.</param>
        /// <remarks>The lock is released when the transaction commits or rolls back.</remarks>
        public static void AcquireLock(IDbTransaction transaction, int lockId)
        {
            AcquireLock(transaction, (long)lockId);
        }

        /// <inheritdoc cref="AcquireLock(System.Data.IDbTransaction,int)"/>
        public static void AcquireLock(IDbTransaction transaction, long lockId)
        {
            Lock(transaction, lockId);
        }

        /// <summary>
        /// Releases the advisory lock.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles the actual cleanup work.
        /// </summary>
        /// <param name="disposing">Indicates if it is called from IDisposable.Dispose (true), or false when called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                advisoryLock?.Dispose();
            }
        }

        private static void Lock(IDbTransaction tx, long lockId)
        {
            tx.Connection.Execute($"SELECT pg_advisory_xact_lock(CAST({lockId} AS BIGINT));", transaction: tx);
        }

        private class ManagedConnectionAdvisoryLock : IDisposable
        {
            private readonly NpgsqlConnection connection;
            private readonly SessionAdvisoryLock sessionAdvisoryLock;

            public ManagedConnectionAdvisoryLock(string connectionString, long lockId)
            {
                var connectionStringWithoutPooling = BuildConnectionStringWithoutPooling(connectionString);
                connection = new NpgsqlConnection(connectionStringWithoutPooling);
                connection.Open();
                sessionAdvisoryLock = new SessionAdvisoryLock(connection, lockId);
            }

            /// <summary>
            /// Releases the advisory lock.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Handles the actual cleanup work.
            /// </summary>
            /// <param name="disposing">Indicates if it is called from IDisposable.Dispose (true), or false when called from finalizer.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    sessionAdvisoryLock?.Dispose();
                    connection?.Dispose();
                }
            }

            private static string BuildConnectionStringWithoutPooling(string connectionString)
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString) { Pooling = false };
                return builder.ConnectionString;
            }
        }

        private class SessionAdvisoryLock : IDisposable
        {
            private readonly IDbConnection connection;
            private readonly long lockId;

            public SessionAdvisoryLock(IDbConnection connection, long lockId)
            {
                this.connection = connection;
                this.lockId = lockId;
                LockSession(connection, lockId);
            }

            ~SessionAdvisoryLock()
            {
                Dispose(false);
            }

            /// <summary>
            /// Releases the advisory lock.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Handles the actual cleanup work.
            /// </summary>
            /// <param name="disposing">Indicates if it is called from IDisposable.Dispose (true), or false when called from finalizer.</param>
            protected virtual void Dispose(bool disposing)
            {
                ReleaseUnmanagedResources();
            }

            private static void LockSession(IDbConnection connection, long lockId)
            {
                connection.Execute($"SELECT pg_advisory_lock(CAST({lockId} AS BIGINT));");
            }

            private static void Unlock(IDbConnection connection, long lockId)
            {
                switch (connection.State)
                {
                    case ConnectionState.Connecting:
                    case ConnectionState.Executing:
                    case ConnectionState.Fetching:
                    case ConnectionState.Open:
                        connection.Execute($"SELECT pg_advisory_unlock(CAST({lockId} AS BIGINT));");
                        break;
                    case ConnectionState.Broken:
                    case ConnectionState.Closed:
                        break;
                    default:
                        throw new NotSupportedException($"The connection state {connection.State} is not supported.");
                }
            }

            private void ReleaseUnmanagedResources()
            {
                Unlock(connection, lockId);
            }
        }
    }
}
