using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using RedLine.Domain;

namespace RedLine.Extensions.Hosting.Web
{
    internal static class HttpContextExtensions
    {
        internal static string AccessToken(this HttpContext httpContext)
        {
            return httpContext
                ?.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        internal static Guid CorrelationId(this HttpContext httpContext)
        {
            var headerValue = httpContext.Request
                .Headers[RedLineHeaderNames.CorrelationId]
                .FirstOrDefault();

            return Guid.TryParse(headerValue, out Guid result)
                ? result
                : Guid.NewGuid();
        }

        internal static string ETag(this HttpContext httpContext)
        {
            var httpHeaders = httpContext.Request?.Headers;

            string etag = null;

            if (httpHeaders != null)
            {
                if (httpHeaders.ContainsKey(HeaderNames.IfMatch))
                {
                    etag = httpHeaders[HeaderNames.IfMatch].First();
                }
                else if (httpHeaders.ContainsKey(HeaderNames.IfNoneMatch))
                {
                    etag = httpHeaders[HeaderNames.IfNoneMatch].First();
                }
            }

            return etag;
        }

        internal static string TenantId(this HttpContext httpContext)
        {
            var routeValues = httpContext.GetRouteData().Values;

            var tenantId = routeValues.ContainsKey("tenantId")
                ? routeValues["tenantId"].ToString()
                : string.Empty;

            return tenantId;
        }

        internal static string RemoteIpAddress(this HttpContext httpContext)
        {
            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }
    }
}
