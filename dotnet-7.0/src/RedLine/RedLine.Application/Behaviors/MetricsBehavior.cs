using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Meter;
using App.Metrics.Timer;
using MediatR;
using RedLine.Domain;

namespace RedLine.Application.Behaviors
{
    /// <summary>
    /// A behavior used to publish metrics for each request.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    internal class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        /// <summary>
        /// The metrics instance we write to.
        /// </summary>
        private readonly IMetrics metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="metrics">The metrics instance we write to.</param>
        public MetricsBehavior(IMetrics metrics)
        {
            this.metrics = metrics;
        }

        /// <summary>
        /// Writes metrics about the current request.
        /// </summary>
        /// <param name="request">The request to authorize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <returns>The response from the request handler.</returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var requestType = (request as IActivity)?.ActivityType.ToString() ?? "Unknown";
            var requestName = TypeNameHelper.GetTypeDisplayName(typeof(TRequest), false);

            var tags = new MetricTags(
                new[] { "request_type", "request_name" },
                new[] { requestType, requestName });

            using (metrics.Measure.Timer.Time(Metrics.k8s_scaling_request_duration_seconds)) // Don't put tags on this one
            using (metrics.Measure.Timer.Time(Metrics.request_duration_seconds, tags))
            {
                try
                {
                    metrics.Measure.Counter.Increment(Metrics.requests_total, tags);
                    metrics.Measure.Meter.Mark(Metrics.requests_per_second, tags);
                    metrics.Measure.Meter.Mark(Metrics.k8s_scaling_requests_per_second); // Don't put tags on this one

                    return await next().ConfigureAwait(false);
                }
                catch
                {
                    metrics.Measure.Counter.Increment(Metrics.request_errors_total, tags);
                    metrics.Measure.Meter.Mark(Metrics.request_errors_per_second, tags);

                    throw;
                }
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1311:Static readonly fields should begin with upper-case letter", Justification = "This is a special class and needs to be this way for our Prometheus tooling.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "This is a special class and needs to be this way for our Prometheus tooling.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "This is a special case.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "This is a special case.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "This is special case.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1304:Non-private readonly fields should begin with upper-case letter", Justification = "This is special case.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "This is a special case.")]
    internal static class Metrics
    {
        /* IMPORTANT:
         * We have tooling looking for these metrics. Do not change their names unless you change the tooling
         * as well. FYI: these names, and the units, are based on the Prometheus standards.
         */
        private static readonly string context = ApplicationContext.ApplicationSystemName.Replace('-', '_');

        // Used for Kubernetes auto-scaling
        internal static readonly MeterOptions k8s_scaling_requests_per_second = new MeterOptions
        {
            Context = context,
            Name = nameof(k8s_scaling_requests_per_second),
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
        };

        // Used for Kubernetes auto-scaling
        internal static readonly TimerOptions k8s_scaling_request_duration_seconds = new TimerOptions
        {
            Context = context,
            Name = nameof(k8s_scaling_request_duration_seconds),
            MeasurementUnit = App.Metrics.Unit.Requests,
            DurationUnit = TimeUnit.Seconds,
            RateUnit = TimeUnit.Seconds,
        };

        // Per activity execution metrics
        internal static readonly TimerOptions request_duration_seconds = new TimerOptions
        {
            Context = context,
            Name = nameof(request_duration_seconds),
            MeasurementUnit = App.Metrics.Unit.Requests,
            DurationUnit = TimeUnit.Seconds,
            RateUnit = TimeUnit.Seconds,
        };

        // Per activity execution metrics
        internal static readonly MeterOptions requests_per_second = new MeterOptions
        {
            Context = context,
            Name = nameof(requests_per_second),
            MeasurementUnit = App.Metrics.Unit.Requests,
            RateUnit = TimeUnit.Seconds,
        };

        // Per activity execution metrics
        internal static readonly CounterOptions requests_total = new CounterOptions
        {
            Context = context,
            Name = nameof(requests_total),
            MeasurementUnit = App.Metrics.Unit.Requests,
        };

        // Per activity error metrics
        internal static readonly MeterOptions request_errors_per_second = new MeterOptions
        {
            Context = context,
            Name = nameof(request_errors_per_second),
            MeasurementUnit = App.Metrics.Unit.Errors,
            RateUnit = TimeUnit.Seconds,
        };

        // Per activity error metrics
        internal static readonly CounterOptions request_errors_total = new CounterOptions
        {
            Context = context,
            Name = nameof(request_errors_total),
            MeasurementUnit = App.Metrics.Unit.Errors,
        };
    }
}
