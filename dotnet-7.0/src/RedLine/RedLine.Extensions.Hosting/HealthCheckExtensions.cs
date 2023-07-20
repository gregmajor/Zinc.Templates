using System;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Data;
using RedLine.Domain;
using RedLine.HealthChecks;

namespace RedLine.Extensions.Hosting
{
    /// <summary>
    /// Provides health check extension methods for <see cref="IServiceCollection"/> and <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class HealthCheckExtensions
    {
        private static readonly byte[] EmptyResponse = new byte[] { (byte)'{', (byte)'}' };

        /// <summary>
        /// Adds the standard RedLine health checks to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The application configuration settings.</param>
        /// <param name="withCustomHealthChecks">A delegate used to add custom health checks.</param>
        /// <param name="configureOptions">A delegate to change the default options.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IHealthChecksBuilder> withCustomHealthChecks,
            Action<HealthCheckChoices> configureOptions)
        {
            withCustomHealthChecks?.Invoke(services.AddHealthChecks());

            var options = new HealthCheckChoices();
            configureOptions?.Invoke(options);

            if (!options.DisableMemoryCheck)
            {
                services.AddMemoryHealthChecks();
            }

            if (!options.DisableAuthZCheck)
            {
                services.AddUrlGroupHealthChecks(
                    "AuthZ Service",
                    TimeSpan.FromSeconds(5),
                    new Uri(new Uri(ApplicationContext.AuthorizationServiceEndpoint), ".well-known/ready"));
            }

            if (!options.DisableAuthNCertCheck)
            {
                services.AddCertificateExpirationHealthCheck(
                    "AuthN Certificate",
                    TimeSpan.FromSeconds(5),
                    ApplicationContext.AuthenticationServicePublicKeyPath,
                    TimeSpan.FromDays(60),
                    TimeSpan.FromDays(5));
            }

            if (!options.DisablePostgresCheck)
            {
                services.AddPostgresHealthCheck(
                    "Database",
                    TimeSpan.FromSeconds(5),
                    new PostgresConnectionString(configuration).Value);
            }

            if (!options.DisableRabbitCheck)
            {
                services.AddRabbitMqHealthCheck(
                    "RabbitMQ Connection",
                    TimeSpan.FromSeconds(5));
            }

            return services;
        }

        /// <summary>
        /// Adds the standard RedLine health checks to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The application configuration settings.</param>
        /// <param name="withCustomHealthChecks">A delegate used to add custom health checks.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IHealthChecksBuilder> withCustomHealthChecks) => services.AddRedLineHealthChecks(configuration, withCustomHealthChecks, null);

        /// <summary>
        /// Adds the standard RedLine health checks to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The application configuration settings.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration) => services.AddRedLineHealthChecks(configuration, null, null);

        /// <summary>
        /// Configures the /.well-known/ready and /.well-known/live endpoints.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRedLineHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/.well-known/ready", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = (httpContext, report) =>
                {
                    httpContext.Response.ContentType = MediaTypeNames.Application.Json;

                    if (report != null)
                    {
                        var response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
                            report,
                            HealthChecks.HealthCheckJsonSerializerSettings.Default));

                        httpContext.Response.Body.Write(response, 0, response.Length);
                    }
                    else
                    {
                        httpContext.Response.Body.Write(EmptyResponse, 0, EmptyResponse.Length);
                    }

                    return Task.CompletedTask;
                },
            });

            app.UseHealthChecks("/.well-known/live", new HealthCheckOptions
            {
                Predicate = _ => false,
                ResponseWriter = (httpContext, _) =>
                {
                    var response = Encoding.UTF8.GetBytes("pong");

                    httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
                    httpContext.Response.Body.Write(response, 0, response.Length);

                    return Task.CompletedTask;
                },
            });

            return app;
        }
    }
}
