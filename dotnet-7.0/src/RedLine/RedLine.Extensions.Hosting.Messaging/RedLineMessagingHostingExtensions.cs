using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RedLine.A3.Authentication;
using RedLine.Application;
using RedLine.Data;
using RedLine.Domain;

namespace RedLine.Extensions.Hosting.Messaging
{
    /// <summary>
    /// Extension methods for hosting a RedLine NServiceBus host.
    /// </summary>
    public static class RedLineMessagingHostingExtensions
    {
        /// <summary>
        /// Adds the RedLine Job Host services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineMessagingHost(this IServiceCollection services)
        {
            return services
                .AddRabbitMQ()
                .AddRedLineDataServices()
                .AddRedLineApplicationServices()
                .AddJwtServiceAuthentication()
                .AddTenantId()
                .AddCorrelationId()
                .AddETag()
                .AddClientAddress()
                .AddMetrics()
                .AddMetricsEndpoints(options =>
                {
                    options.MetricsEndpointOutputFormatter = new MetricsPrometheusFormatter();
                    options.MetricsTextEndpointOutputFormatter = new MetricsJsonOutputFormatter();
                })
                ;
        }

        /// <summary>
        /// Enables the RedLine middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRedLineMessagingHost(this IApplicationBuilder app)
        {
            return app
                .UseMetricsEndpoint()
                .UseMetricsTextEndpoint()
                .UseRedLineHealthChecks()
                ;
        }

        /// <summary>
        /// Adds the RedLine <see cref="IAuthenticationToken"/> to the container.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddJwtServiceAuthentication(this IServiceCollection services)
        {
            services
                .AddScoped<IAuthenticationToken>(container =>
                {
                    return container
                        .GetRequiredService<IAuthenticationTokenProvider>()
                        .GetServiceAuthenticationToken()
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                })
                ;

            return services;
        }

        /// <summary>
        /// Adds the RedLine <see cref="ITenantId"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddTenantId(this IServiceCollection services)
        {
            /* NOTE:
             * We use a wildcard (*) for the tenant here, because the messaging host runs for all tenants, and the
             * wildcard is a convention we already have to indicate any or all. That said, if a message comes in
             * with a redline-tenant-id header, that tenant will be used in lieu of the wildcard, which shouldn't
             * be an issue because the service account has permissions across tenants. The problem arises when there
             * is no tenant at all, in which case the service account cannot obtain its grants from AuthZ.
             * */
            return services.AddScoped<ITenantId, TenantId>(_ => new TenantId("*"));
        }

        /// <summary>
        /// Adds the RedLine <see cref="ICorrelationId"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddCorrelationId(this IServiceCollection services)
        {
            return services.AddScoped<ICorrelationId, CorrelationId>(_ => new CorrelationId());
        }

        /// <summary>
        /// Adds the RedLine <see cref="IETag"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddETag(this IServiceCollection services)
        {
            return services.AddScoped<IETag, ETag>(_ => new ETag());
        }

        /// <summary>
        /// Adds the RedLine <see cref="IClientAddress"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddClientAddress(this IServiceCollection services)
        {
            /* NOTE:
             * The IncomingRedLineHeadersBehavior class sets the value of ClientAddress to be
             * the same as the 'NServiceBus.OriginatingEndpoint' header value, which is the
             * endpoint name of the message originator.
             * */
            return services.AddScoped<IClientAddress, ClientAddress>(_ => new ClientAddress());
        }
    }
}
