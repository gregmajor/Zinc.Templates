using System;
using Npgsql.Logging;
using NServiceBus.Logging;

namespace Zinc.Templates.FunctionalTests.Logging
{
    internal class PostgresTestLogger : NpgsqlLogger
    {
        private readonly PostgresTestLoggingProvider provider;

        internal PostgresTestLogger(PostgresTestLoggingProvider provider)
        {
            this.provider = provider;
        }

        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return true;
        }

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            var logLevel = ToNLogLogLevel(level);
            if (exception != null)
            {
                provider.TestOutputHelper?.WriteLine($"{logLevel} - {msg} - {exception.ToString()}");
            }
            else
            {
                provider.TestOutputHelper?.WriteLine($"{logLevel} - {msg}");
            }
        }

        private static LogLevel ToNLogLogLevel(NpgsqlLogLevel level)
        {
            return level switch
            {
                NpgsqlLogLevel.Trace => LogLevel.Debug,
                NpgsqlLogLevel.Debug => LogLevel.Debug,
                NpgsqlLogLevel.Info => LogLevel.Info,
                NpgsqlLogLevel.Warn => LogLevel.Warn,
                NpgsqlLogLevel.Error => LogLevel.Error,
                NpgsqlLogLevel.Fatal => LogLevel.Fatal,
                _ => throw new ArgumentOutOfRangeException(nameof(level)),
            };
        }
    }
}
