using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Krypton.Audit;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using RedLine.A3.Audit;
using RedLine.Application.Jobs;
using RedLine.Data.Outbox;
using RedLine.Domain;

namespace RedLine.Application.Behaviors
{
    /// <summary>
    /// A behavior that audits activity execution.
    /// </summary>
    /// <typeparam name="TRequest">The type of command or query being executed.</typeparam>
    /// <typeparam name="TResponse">The response type returned from the command or query.</typeparam>
    internal class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IActivityContext context;
        private readonly IMessageSession bus;
        private readonly ILogger<AuditBehavior<TRequest, TResponse>> logger;
        private readonly IOutbox outbox;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/> for the current request.</param>
        /// <param name="bus">The NSB <see cref="IMessageSession"/> used to publish the event.</param>
        /// <param name="logger">A diagnostic logger.</param>
        /// <param name="outbox">The <see cref="IOutbox"/>, which is used to save the event if/when Rabbit is down.</param>
        public AuditBehavior(
            IActivityContext context,
            IMessageSession bus,
            ILogger<AuditBehavior<TRequest, TResponse>> logger,
            IOutbox outbox)
        {
            this.context = context;
            this.bus = bus;
            this.logger = logger;
            this.outbox = outbox;
        }

        /// <summary>
        /// Applies the behavior and executes the next one in the pipeline.
        /// </summary>
        /// <param name="request">The executing request.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the request.</param>
        /// <param name="next">The next behavior in the pipeline.</param>
        /// <returns>The response as a <typeparamref name="TResponse"/>.</returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            /* NOTE:
             * We do not audit activities that do not implement IAmAuditable, which frankly should never occur.
             * */
            if (request is not IAmAuditable)
            {
                return await next().ConfigureAwait(false);
            }

            /* NOTE:
             * The ActivityAudit is a Decorator that wraps the activity execution in order to collect
             * audit details from the request/response, including any exception that may occur.
             * */
            var activityAudit = new ActivityAudit<TRequest, TResponse>(context, request);

            try
            {
                return await activityAudit.Decorate(next.Invoke).ConfigureAwait(false);
            }
            finally
            {
                if (!IsJobWithNoWorkPerformed(activityAudit.Response))
                {
                    var activityAudited = new ActivityAudited(activityAudit);

                    try
                    {
                        await bus.Publish(activityAudited, context).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        /* Oops, we couldn't dispatch the event! Let's at least log it
                         * so there is some record of it, and then save it to the Outbox
                         * so it can be published later.
                         * */
                        LogFailure(activityAudited, e);
                        await SaveToOutbox(activityAudited).ConfigureAwait(false);
                    }
                }
            }
        }

        private void LogFailure(ActivityAudited activityAudited, Exception e)
        {
            var dictionary = new Dictionary<string, object>
            {
                { nameof(CorrelationId), activityAudited.CorrelationId },
                { nameof(TenantId), activityAudited.TenantId },
            };

            using (logger.BeginScope(dictionary))
            {
                logger.LogCritical(
                    e,
                    "[AUDIT]==> {Error} occurred while publishing an ActivityAudited event! The event will be queued to the Outbox.\n{ApplicationName}/{ActivityName}?u={UserName}\n[AUDIT]<== ERROR 500: {Message}",
                    e.GetType().Name,
                    activityAudited.ApplicationName,
                    activityAudited.ActivityName,
                    activityAudited.UserName,
                    e.Message);
            }
        }

        private async Task SaveToOutbox(ActivityAudited activityAudited)
        {
            var message = new OutboxMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                MessageBody = activityAudited,
                MessageHeaders = new Dictionary<string, string>
                {
                    { RedLineHeaderNames.CorrelationId, activityAudited.CorrelationId.ToString() },
                    { RedLineHeaderNames.TenantId, activityAudited.TenantId },
                },
            };

            await outbox.SaveMessages(new[] { message }).ConfigureAwait(false);
        }

        private bool IsJobWithNoWorkPerformed(TResponse response)
        {
            return JobResult.NoWorkPerformed.Equals(response);
        }
    }
}
