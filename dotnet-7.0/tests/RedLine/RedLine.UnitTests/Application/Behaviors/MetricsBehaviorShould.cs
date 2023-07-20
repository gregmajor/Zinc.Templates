using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Meter;
using App.Metrics.Timer;
using FluentAssertions;
using Moq;
using RedLine.Application.Behaviors;
using Xunit;
using CustomMetrics = RedLine.Application.Behaviors.Metrics;

namespace RedLine.UnitTests.Application.Behaviors
{
    [Collection(nameof(UnitTestCollection))]
    public class MetricsBehaviorShould
    {
        private readonly Mock<IMetrics> metricsMock = new();
        private readonly Mock<IMeasureMetrics> measureMock = new();
        private readonly Mock<IMeasureCounterMetrics> counterMock = new();
        private readonly Mock<IMeasureMeterMetrics> meterMock = new();
        private readonly Mock<IMeasureTimerMetrics> timerMock = new();
        private readonly Mock<IDisposable> disposableMock = new();
        private readonly object request = new();
        private readonly object response = new();

        [SuppressMessage("StyleCop", "SA1129", Justification = "No arg constructor is needed for brevity.")]
        public MetricsBehaviorShould()
        {
            metricsMock.Setup(x => x.Measure).Returns(measureMock.Object);
            measureMock.Setup(x => x.Counter).Returns(counterMock.Object);
            measureMock.Setup(x => x.Meter).Returns(meterMock.Object);
            measureMock.Setup(x => x.Timer).Returns(timerMock.Object);
            timerMock.Setup(x => x.Time(It.IsAny<TimerOptions>())).Returns(new TimerContext());
            timerMock.Setup(x => x.Time(It.IsAny<TimerOptions>(), It.IsAny<MetricTags>())).Returns(new TimerContext());
            counterMock.Setup(x => x.Increment(It.IsAny<CounterOptions>(), It.IsAny<MetricTags>()));
            meterMock.Setup(x => x.Mark(It.IsAny<MeterOptions>()));
            meterMock.Setup(x => x.Mark(It.IsAny<MeterOptions>(), It.IsAny<MetricTags>()));
        }

        [Fact]
        public async Task CollectHappyPathMetrics()
        {
            // Arrange
            MetricsBehavior<object, object> behavior = new(metricsMock.Object);

            // Act
            await behavior.Handle(request, CancellationToken.None, Next).ConfigureAwait(false);

            // Assert
            timerMock.Verify(x => x.Time(CustomMetrics.k8s_scaling_request_duration_seconds));
            timerMock.Verify(x => x.Time(CustomMetrics.request_duration_seconds, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            counterMock.Verify(x => x.Increment(CustomMetrics.requests_total, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            meterMock.Verify(x => x.Mark(CustomMetrics.requests_per_second, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            meterMock.Verify(x => x.Mark(CustomMetrics.k8s_scaling_requests_per_second));
        }

        [Fact]
        public async Task CollectFailureMetrics()
        {
            // Arrange
            MetricsBehavior<object, object> behavior = new(metricsMock.Object);

            // Act
            Func<Task<object>> action = () => behavior.Handle(request, CancellationToken.None, NextException);

            // Assert
            await action.Should().ThrowAsync<Exception>().ConfigureAwait(false);
            timerMock.Verify(x => x.Time(CustomMetrics.k8s_scaling_request_duration_seconds));
            timerMock.Verify(x => x.Time(CustomMetrics.request_duration_seconds, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            counterMock.Verify(x => x.Increment(CustomMetrics.requests_total, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            meterMock.Verify(x => x.Mark(CustomMetrics.requests_per_second, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            meterMock.Verify(x => x.Mark(CustomMetrics.k8s_scaling_requests_per_second));
            counterMock.Verify(x => x.Increment(CustomMetrics.request_errors_total, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
            meterMock.Verify(x => x.Mark(CustomMetrics.request_errors_per_second, It.Is<MetricTags>(t => ExpectedMetricTags(t))));
        }

        private bool ExpectedMetricTags(MetricTags tags)
        {
            var data = tags.ToDictionary();
            return data["request_type"] == "Unknown"
                && data["request_name"] == TypeNameHelper.GetTypeDisplayName(request.GetType());
        }

        private Task<object> Next() => Task.FromResult(response);

        private Task<object> NextException() => throw new Exception(nameof(NextException));
    }
}
