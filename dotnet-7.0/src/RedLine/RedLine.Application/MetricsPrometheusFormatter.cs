using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters;
using App.Metrics.Formatters.Prometheus;
using App.Metrics.Formatters.Prometheus.Internal;
using App.Metrics.Gauge;
using App.Metrics.Meter;
using RedLine.Domain;

namespace RedLine.Application
{
    /// <summary>
    /// <para>
    /// This class serves to properly write out a Meter metric type because the built-in formatter does not handle
    /// the Meter type very well (it writes a count instead of a rate).
    /// </para>
    /// <para>TODO A better approach is to fix App.Metrics Prometheus formatter and make a pull request.</para>
    /// </summary>
    public class MetricsPrometheusFormatter : IMetricsOutputFormatter
    {
        /// <summary>
        /// The context for these metrics.
        /// </summary>
        private static readonly string ContextName = ApplicationContext.ApplicationSystemName.Replace('-', '_');

        /// <summary>
        /// The built-in Prometheus formatter we delegate to for most things.
        /// </summary>
        private readonly MetricsPrometheusTextOutputFormatter delegateFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsPrometheusFormatter"/> class.
        /// </summary>
        public MetricsPrometheusFormatter()
        {
            /* HACK
             *  The built-in web metrics that provide cool things like Apdex tracking currently hard code their
             *  context to 'Application.HttpRequests' (or 'application_httprequests' in Prometheus naming standard).
             *  Unfortunately, this means ALL of our apps/services would be reporting using this name and thus
             *  the metrics would be worthless because they could not be separated by application. As a result,
             *  we have to manually change the context from 'application_httprequests' to '<app_name>_httprequests'
             *  before we publish the metrics to Prometheus. THIS CODE SHOULD BE REMOVED ONCE App.Metrics supports
             *  the desired behavior. Or better yet, fix it for them and create a pull request.
             *
             *  NOTE: This hack may have been fixed in the latest App.Metrics code, but I'm too lazy to check.
             */
            var options = new MetricsPrometheusOptions
            {
                MetricNameFormatter = (context, metric) =>
                {
                    var result = PrometheusFormatterConstants.MetricNameFormatter(context, metric);

                    if (result.StartsWith("application_httprequests", StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = result.Replace("application_httprequests", ContextName, StringComparison.InvariantCultureIgnoreCase);
                    }

                    return result;
                },
            };

            delegateFormatter = new MetricsPrometheusTextOutputFormatter(options);
        }

        /// <summary>
        /// Gets the <see cref="MetricsMediaTypeValue"/> of the formatter.
        /// </summary>
        public MetricsMediaTypeValue MediaType => delegateFormatter.MediaType;

        /// <summary>
        /// Gets or sets the <see cref="MetricFields"/> of the formatter.
        /// </summary>
        public MetricFields MetricFields
        {
            get => delegateFormatter.MetricFields;
            set => delegateFormatter.MetricFields = value;
        }

        /// <summary>
        /// Writes the <see cref="MetricsDataValueSource"/> to the given stream.
        /// </summary>
        /// <param name="output">The stream to write the output to.</param>
        /// <param name="metricsData">The <see cref="MetricsDataValueSource"/> to write.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
        /// <returns>A task used to await the operation.</returns>
        public async Task WriteAsync(Stream output, MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            await delegateFormatter.WriteAsync(
                output,
                new MetricsDataValueSource(metricsData.Timestamp, ConvertMetersToGauges(metricsData.Contexts)),
                cancellationToken).ConfigureAwait(false);
        }

        private IEnumerable<MetricsContextValueSource> ConvertMetersToGauges(IEnumerable<MetricsContextValueSource> contexts)
        {
            foreach (var context in contexts)
            {
                var values = new MetricsContextValueSource(
                    context.Context,
                    context.Gauges.Concat(ConvertMetersToGauges(context.Meters)), // Convert meters to gauges and concatenate them with any existing gauges
                    context.Counters,
                    Enumerable.Empty<MeterValueSource>(), // Clear out any meters
                    context.Histograms,
                    context.BucketHistograms,
                    context.Timers,
                    context.BucketTimers,
                    context.ApdexScores);

                yield return values;
            }
        }

        private IEnumerable<GaugeValueSource> ConvertMetersToGauges(IEnumerable<MeterValueSource> meters)
        {
            /* NOTE
             *  A meter exposes a number of values, a "mean" value, and three "exponential moving average"
             *  values over time spans of 1 min, 5 min, and 15 min. The moving averages give rise to a "recency"
             *  in the movements. For example, you may see a surge in the 1min average but not in the 15min average.
             *  This indicates the surge was recent. As a result, for each meter, we create four gauges, one for
             *  the mean and one for each of the exponential moving averages.
             */
            foreach (var meter in meters)
            {
                yield return new GaugeValueSource(
                    string.Concat(meter.IsMultidimensional ? meter.MultidimensionalName : meter.Name, "_rate"),
                    ConstantValue.Provider(meter.Value.MeanRate),
                    Unit.Custom($"{meter.Unit.Name}/{meter.RateUnit.Unit()}"),
                    meter.Tags);

                yield return new GaugeValueSource(
                    string.Concat(meter.IsMultidimensional ? meter.MultidimensionalName : meter.Name, "_rate1m"),
                    ConstantValue.Provider(meter.Value.OneMinuteRate),
                    Unit.Custom($"{meter.Unit.Name}/{meter.RateUnit.Unit()}"),
                    meter.Tags);

                yield return new GaugeValueSource(
                    string.Concat(meter.IsMultidimensional ? meter.MultidimensionalName : meter.Name, "_rate5m"),
                    ConstantValue.Provider(meter.Value.FiveMinuteRate),
                    Unit.Custom($"{meter.Unit.Name}/{meter.RateUnit.Unit()}"),
                    meter.Tags);

                yield return new GaugeValueSource(
                    string.Concat(meter.IsMultidimensional ? meter.MultidimensionalName : meter.Name, "_rate15m"),
                    ConstantValue.Provider(meter.Value.FifteenMinuteRate),
                    Unit.Custom($"{meter.Unit.Name}/{meter.RateUnit.Unit()}"),
                    meter.Tags);
            }
        }
    }
}
