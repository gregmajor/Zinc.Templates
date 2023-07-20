using System;
using System.Linq;
using System.Net;
using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RedLine.A3.Authentication;
using RedLine.Application;
using RedLine.Data;
using RedLine.Domain;

namespace RedLine.Extensions.Hosting.Jobs
{
    /// <summary>
    /// Extension methods for hosting a RedLine website or API.
    /// </summary>
    public static class RedLineJobHostingExtensions
    {
        private static readonly string DefaultSchedulerId = Guid.NewGuid().ToString();

        /// <summary>
        /// Adds the RedLine Job Host services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <param name="withJobs">A delegate used to configure the jobs.</param>
        /// <param name="schedulerId">The optional scheduler id.</param>
        /// <param name="schedulerName">The optional scheduler name.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineJobHost(
            this IServiceCollection services,
            Action<IServiceCollectionQuartzConfigurator> withJobs,
            string schedulerId,
            string schedulerName)
        {
            return services
                .AddRabbitMQ()
                .AddRedLineDataServices()
                .AddRedLineApplicationServices()
                .AddQuartzServices(withJobs, schedulerId, schedulerName)
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
        /// Adds the RedLine Job Host services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <param name="withJobs">A delegate used to configure the jobs.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineJobHost(
            this IServiceCollection services,
            Action<IServiceCollectionQuartzConfigurator> withJobs)
            => services.AddRedLineJobHost(withJobs, null, null);

        /// <summary>
        /// Enables the RedLine middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRedLineJobHost(this IApplicationBuilder app)
        {
            return app
                .UseMetricsEndpoint()
                .UseMetricsTextEndpoint()
                .UseRedLineHealthChecks()
                ;
        }

        /// <summary>
        /// Adds the services required for the Jobs Host.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <param name="withJobs">An action used to configure the jobs to execute.</param>
        /// <param name="schedulerId">The optional scheduler id.</param>
        /// <param name="schedulerName">The optional scheduler name.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddQuartzServices(
            this IServiceCollection services,
            Action<IServiceCollectionQuartzConfigurator> withJobs,
            string schedulerId = null,
            string schedulerName = null)
        {
            if (string.IsNullOrEmpty(schedulerId))
            {
                schedulerId = DefaultSchedulerId;
            }

            if (string.IsNullOrEmpty(schedulerName))
            {
                schedulerName = $"{ApplicationContext.ApplicationName}.JobScheduler";
            }

            services.AddQuartz(quartzConfig =>
            {
                quartzConfig.SchedulerId = schedulerId;
                quartzConfig.SchedulerName = schedulerName;
                quartzConfig.UseMicrosoftDependencyInjectionJobFactory();

                withJobs?.Invoke(quartzConfig);
            });

            services.AddQuartzHostedService(config => config.WaitForJobsToComplete = true);

            return services;
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
             * We use a wildcard (*) for the tenant here, because a job can execute under any tenant,
             * and the wildcard is a convention we already have to indicate any or all.
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
            return services.AddSingleton<IClientAddress, ClientAddress>(_ =>
            {
                try
                {
                    var ip = Dns.GetHostAddresses(Dns.GetHostName()).First().ToString();
                    return new ClientAddress(ip);
                }
                catch
                {
                    return new ClientAddress("127.0.0.1");
                }
            });
        }
    }
}
