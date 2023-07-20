using System;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Meter;
using App.Metrics.Timer;
using Moq;
using NServiceBus;
using RedLine.A3;
using RedLine.A3.Authentication;

namespace Zinc.Templates.FunctionalTests
{
    internal static class Mocks
    {
        public static IAccessToken AccessToken => AccessTokenMock.Value;

        public static IMessageSession MessageSession => MessageSessionMock.Value;

        public static IMetrics Metrics => MetricsMock.Value;

        private static Lazy<IAccessToken> AccessTokenMock { get; } = new Lazy<IAccessToken>(() =>
        {
            var mock = new Mock<IAccessToken>();
            mock.Setup(a => a.AuthenticationState).Returns(AuthenticationState.Authenticated);
            mock.Setup(a => a.IsAuthorized(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.Login).Returns(WellKnownId.LocalUser);
            mock.Setup(a => a.TenantId).Returns(WellKnownId.TestTenant);
            mock.Setup(a => a.UserId).Returns(WellKnownId.LocalUser);
            mock.Setup(a => a.FullName).Returns("Fake User");
            return mock.Object;
        });

        private static Lazy<IMessageSession> MessageSessionMock { get; } = new Lazy<IMessageSession>(() =>
        {
            var mock = new Mock<IMessageSession>();

            return mock.Object;
        });

        private static Lazy<IMetrics> MetricsMock { get; } = new Lazy<IMetrics>(() =>
        {
            var mockMeasure = new Mock<IMeasureMetrics>();

            var mockTimer = new Mock<IMeasureTimerMetrics>();
            mockMeasure.SetupGet(x => x.Timer).Returns(mockTimer.Object);

            var mockCounter = new Mock<IMeasureCounterMetrics>();
            mockMeasure.SetupGet(x => x.Counter).Returns(mockCounter.Object);

            var mockMeter = new Mock<IMeasureMeterMetrics>();
            mockMeasure.SetupGet(x => x.Meter).Returns(mockMeter.Object);

            var mock = new Mock<IMetrics>();
            mock.SetupGet(x => x.Measure).Returns(mockMeasure.Object);

            return mock.Object;
        });
    }
}
