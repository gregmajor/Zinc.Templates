using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using RedLine.Domain;
using RedLine.UnitTests.Application.A3.Audit;
using Xunit;

namespace RedLine.UnitTests.Application.MessageSessionExtensionsTests
{
    public class PublishShould
    {
        [Fact]
        public async Task SetHeaders()
        {
            const string userId = "test.user";
            const string userName = "Test User";
            const string incomingEtag = "IncomingEtag";
            const string outgoingEtag = "OutgoingEtag";

            var context = TestActivityContext.NewContext(userId, userName, null);

            context.Get<ETag>(nameof(ETag)).IncomingValue = incomingEtag;
            context.Get<ETag>(nameof(ETag)).OutgoingValue = outgoingEtag;

            var messageSession = new Mock<IMessageSession>();
            PublishOptions options = null;
            messageSession.Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()))
                .Callback<object, PublishOptions>((m, o) => options = o);

            await messageSession.Object.Publish("test", context).ConfigureAwait(false);
            var headers = options.GetHeaders();

            headers[RedLineHeaderNames.TenantId].Should().Be(context.TenantId());
            headers[RedLineHeaderNames.CorrelationId].Should().Be(context.CorrelationId().ToString());
            headers[RedLineHeaderNames.ETag].Should().Be(outgoingEtag);
        }
    }
}
