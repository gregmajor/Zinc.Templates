using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using RedLine.Domain;
using RedLine.UnitTests.Application.A3.Audit;
using Xunit;

namespace RedLine.UnitTests.Application.MessageSessionExtensionsTests
{
    public class SendToThisEndpointShould
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
            SendOptions options = null;
            messageSession.Setup(x => x.Send(It.IsAny<object>(),  It.IsAny<SendOptions>()))
                .Callback<object, SendOptions>((m, o) => options = o);

            await messageSession.Object.SendToThisEndpoint("test", context).ConfigureAwait(false);
            var headers = options.GetHeaders();

            options.IsRoutingToThisEndpoint().Should().BeTrue();
            headers[RedLineHeaderNames.TenantId].Should().Be(context.TenantId());
            headers[RedLineHeaderNames.CorrelationId].Should().Be(context.CorrelationId().ToString());
            headers[RedLineHeaderNames.ETag].Should().Be(outgoingEtag);
        }
    }
}
