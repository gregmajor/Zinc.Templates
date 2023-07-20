using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLine.Application.Jobs.Outbox;
using RedLine.Domain;
using Zinc.Templates.Host.Jobs.Configuration;
using Zinc.Templates.Host.Jobs.HealthChecks;

namespace Zinc.Templates.Host.Jobs.Outbox
{
    /// <summary>
    /// A Quartz job used to dispatch outbox messages.
    /// </summary>
    [DisallowConcurrentExecution]
    internal class OutboxJob : IJob
    {
        internal static readonly string SectionName = $"Jobs:{nameof(OutboxJob)}";
        private static readonly string DefaultDispatcherId = Guid.NewGuid().ToString();
        private readonly IMediator mediator;
        private readonly ITenantId tenantId;
        private readonly ICorrelationId correlationId;
        private readonly ILogger<OutboxJob> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">The <see cref="IMediator"/> used to send the <see cref="DispatchOutboxMessagesJob"/>.</param>
        /// <param name="tenantId">The <see cref="ITenantId"/> for the request, which for this service is "*" since jobs can run for any tenant.</param>
        /// <param name="correlationId">The <see cref="ICorrelationId"/> for the request.</param>
        /// <param name="logger">A diagnostic logger.</param>
        public OutboxJob(
            IMediator mediator,
            ITenantId tenantId,
            ICorrelationId correlationId,
            ILogger<OutboxJob> logger)
        {
            this.mediator = mediator;
            this.tenantId = tenantId;
            this.correlationId = correlationId;
            this.logger = logger;
        }

        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <param name="context">The <see cref="IJobExecutionContext"/> provided by Quartz.</param>
        /// <returns>A <see cref="Task"/> used to wait the operation.</returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var activity = new DispatchOutboxMessagesJob(tenantId.Value, correlationId.Value, DefaultDispatcherId);

                await mediator.Send(activity).ConfigureAwait(false);

                JobHealthCheck<OutboxJob>.Heartbeat();
            }
            catch (Exception e)
            {
                /* NOTE:
                 * The Quartz documentation recommends NOT throwing exceptions from jobs, because the job will
                 * just get executed again immediately, and will likely throw the same exception. So, following
                 * their best practice guidelines, we swallow the exception and let the job execute at its next
                 * scheduled time. Note also that the JobHealthCheck<OutboxJob>.Heartbeat() call will not be made,
                 * so we will know the job is not executing when our health check alarm bells start going off.
                 * */
                logger.LogError(
                    e,
                    "{Error} executing {Job}: {Message}",
                    e.GetType().Name,
                    GetType().FullName,
                    e.Message);
            }
        }

        /// <summary>
        /// Configures the job.
        /// </summary>
        /// <param name="quartz">The <see cref="IServiceCollectionQuartzConfigurator"/> used to configure the job.</param>
        /// <param name="configuration">The application configuration settings.</param>
        internal static void ConfigureJob(IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration)
        {
            var jobConfig = configuration
                .GetSection(SectionName)
                .Get<JobConfig>();

            if (jobConfig.Disabled)
            {
                return;
            }

            quartz.ScheduleJob<OutboxJob>(
                trigger => trigger
                    .WithIdentity($"{nameof(OutboxJob)}Trigger")
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(15))
                    .WithCronSchedule(jobConfig.CronSchedule)
                    .WithDescription("A cron-based trigger for the dispatch outbox messages job."),
                job => job
                    .WithIdentity(nameof(OutboxJob))
                    .WithDescription("The job used to dispatch outbox messages."));
        }

        /// <summary>
        /// Configures the job health check.
        /// </summary>
        /// <param name="healthChecks">The <see cref="IHealthChecksBuilder"/> used to configure the health check.</param>
        /// <param name="configuration">The application configuration settings.</param>
        internal static void ConfigureHealthCheck(IHealthChecksBuilder healthChecks, IConfiguration configuration)
        {
            var jobConfig = configuration
                .GetSection(SectionName)
                .Get<JobConfig>();

            if (jobConfig.Disabled)
            {
                return;
            }

            healthChecks.AddAsyncCheck(
                typeof(OutboxJob).FullName,
                () => new JobHealthCheck<OutboxJob>(
                    jobConfig.DegradedThreshold,
                    jobConfig.UnhealthyThreshold).CheckAsync());
        }
    }
}
