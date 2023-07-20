using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Zinc.Templates.FunctionalTests.Mothers
{
    public static class MessageHandlerMother
    {
        public static MessageHandler NewHandler() => new MessageHandler(new());

        public static MessageHandler WithSuccessResponse(this MessageHandler handler, string host) =>
            handler.WithSuccessResponse(host, string.Empty);

        public static MessageHandler WithSuccessResponse(this MessageHandler handler, string host, object data) =>
            handler.WithSuccessResponse(host, JsonConvert.SerializeObject(data));

        public static MessageHandler WithSuccessResponse(this MessageHandler handler, string host, string data) =>
            new MessageHandler(new(handler.Hosts) { { host, data } });

        public class MessageHandler : HttpMessageHandler
        {
            public MessageHandler(Dictionary<string, string> hosts)
            {
                Hosts = hosts;
            }

            public Dictionary<string, string> Hosts { get; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (Hosts.TryGetValue(request.RequestUri.Host, out var content))
                {
                    return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(content) });
                }

                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.Forbidden });
            }
        }
    }
}
