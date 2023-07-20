using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using RedLine.Domain;

namespace RedLine.Extensions.Hosting.Messaging.Behaviors
{
    /// <summary>
    /// An NSB Behavior used to populate the <see cref="ITenantId"/> and <see cref="ICorrelationId"/>.
    /// </summary>
    internal class IncomingRedLineHeadersBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        /// <summary>
        /// This behavior sets the <see cref="ITenantId"/> and <see cref="CorrelationId"/>.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        /// <param name="next">The next behavior in the pipeline.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var tenantId = context.Builder.Build<ITenantId>();

            if (context.Headers.ContainsKey(RedLineHeaderNames.TenantId))
            {
                tenantId.Value = context.Headers[RedLineHeaderNames.TenantId];
            }

            var correlationId = context.Builder.Build<ICorrelationId>();

            if (context.Headers.ContainsKey(RedLineHeaderNames.CorrelationId))
            {
                correlationId.Value = new Guid(context.Headers[RedLineHeaderNames.CorrelationId]);
            }

            var etag = context.Builder.Build<IETag>();

            if (context.Headers.ContainsKey(RedLineHeaderNames.ETag))
            {
                etag.IncomingValue = context.Headers[RedLineHeaderNames.ETag];
            }

            var clientAddress = context.Builder.Build<IClientAddress>();

            if (context.Headers.ContainsKey(RedLineHeaderNames.OriginatingEndpoint))
            {
                clientAddress.Value = context.Headers[RedLineHeaderNames.OriginatingEndpoint];
            }

            await next().ConfigureAwait(false);
        }
    }
}
