using System.Threading;
using Npgsql.Logging;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Logging
{
    internal class PostgresTestLoggingProvider : INpgsqlLoggingProvider
    {
        private readonly AsyncLocal<ITestOutputHelper> testOutputHelper = new AsyncLocal<ITestOutputHelper>();

        internal ITestOutputHelper TestOutputHelper => testOutputHelper.Value;

        public NpgsqlLogger CreateLogger(string name)
        {
            return new PostgresTestLogger(this);
        }

        public void Register(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper.Value = testOutputHelper;
        }

        public void Unregister(ITestOutputHelper testOutputHelper)
        {
            if (ReferenceEquals(this.testOutputHelper.Value, testOutputHelper))
            {
                this.testOutputHelper.Value = null;
            }
        }
    }
}
