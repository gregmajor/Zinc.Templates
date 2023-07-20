using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;
using Xunit.Abstractions;

namespace Zinc.Templates.IntegrationTests.Messaging
{
    public class TestOutputSink : ILogEventSink
    {
        private readonly ConcurrentQueue<string> logMessages = new ConcurrentQueue<string>();
        private ITestOutputHelper outputHelper;

        public void Register(ITestOutputHelper output)
        {
            outputHelper = output;
        }

        public void Unregister()
        {
            while (logMessages.TryDequeue(out var message))
            {
                outputHelper.WriteLine(message);
            }
        }

        public void Emit(LogEvent logEvent)
        {
            logMessages.Enqueue(logEvent.RenderMessage());
            if (logEvent.Exception != null)
            {
                logMessages.Enqueue(logEvent.Exception.ToString());
            }
        }
    }
}
