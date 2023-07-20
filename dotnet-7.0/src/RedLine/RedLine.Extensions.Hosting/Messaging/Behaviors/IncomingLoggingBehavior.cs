using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus.Pipeline;
using RedLine.Domain;

namespace RedLine.Extensions.Hosting.Messaging.Behaviors
{
    /// <summary>
    /// NSB Behavior to add a logging context.
    /// </summary>
    internal class IncomingLoggingBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        /// <summary>
        /// This behavior is used to set a logging scope for NSB-specific parts of the pipeline.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <param name="next">The next behavior in the pipeline.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var correlationId = context.Builder.Build<ICorrelationId>();
            var tenantId = context.Builder.Build<ITenantId>();
            var logger = context.Builder.Build<ILogger<IncomingLoggingBehavior>>();

            var dictionary = new Dictionary<string, object>
            {
                { nameof(CorrelationId), correlationId.Value },
                { nameof(TenantId), tenantId.Value },
            };

            using (logger.BeginScope(dictionary))
            {
                await next().ConfigureAwait(false);
            }
        }
    }
}
