using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using RedLine.Domain;

namespace RedLine.Application.Behaviors
{
    /// <summary>
    /// A behavior used to log requests and any exceptions that occur.
    /// </summary>
    /// <typeparam name="TRequest">The type of request.</typeparam>
    /// <typeparam name="TResponse">The type of response.</typeparam>
    internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The diagnostic logger to use.</param>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Logs the current request, as well as any exception that may occur.
        /// </summary>
        /// <param name="request">The request to authorize.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <param name="next">The next request in the pipeline.</param>
        /// <returns>The response from the request handler.</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var dictionary = new Dictionary<string, object>
            {
                { nameof(CorrelationId), (request as IAmCorrelatable)?.CorrelationId ?? Guid.Empty },
                { nameof(TenantId), (request as IAmMultiTenant)?.TenantId ?? string.Empty },
            };

            using (logger.BeginScope(dictionary))
            {
                try
                {
                    var response = await next().ConfigureAwait(false);

                    if (!object.Equals(response, default(TResponse)))
                    {
                        var showValueString =
                            TypeNameHelper.BuiltInTypeNames.ContainsKey(typeof(TResponse))
                            || response is Guid;

                        logger.LogDebug(
                            "[APPL]==> {Request}\n[APPL]<== OK: {Response}",
                            TypeNameHelper.GetTypeDisplayName(typeof(TRequest), false),
                            showValueString ? response.ToString() : TypeNameHelper.GetTypeDisplayName(typeof(TResponse), false));
                    }
                    else
                    {
                        logger.LogDebug(
                            "[APPL]==> {Request}\n[APPL]<== OK",
                            TypeNameHelper.GetTypeDisplayName(typeof(TRequest), false));
                    }

                    return response;
                }
                catch (AggregateException e)
                {
                    logger.LogError(
                        e.InnerException,
                        "[APPL]==> {Request}\n[APPL]<== ERROR {StatusCode}: {Message}",
                        TypeNameHelper.GetTypeDisplayName(typeof(TRequest), false),
                        GetStatusCode(e.InnerException),
                        e.InnerException.Message);

                    throw;
                }
                catch (Exception e)
                {
                    logger.LogError(
                        e,
                        "[APPL]==> {Request}\n[APPL]<== ERROR {StatusCode}: {Message}",
                        TypeNameHelper.GetTypeDisplayName(typeof(TRequest), false),
                        GetStatusCode(e),
                        e.Message);

                    throw;
                }
            }
        }

        private int GetStatusCode(Exception e)
        {
            return (e as Domain.Exceptions.RedLineException)?.StatusCode ?? 500;
        }
    }
}
