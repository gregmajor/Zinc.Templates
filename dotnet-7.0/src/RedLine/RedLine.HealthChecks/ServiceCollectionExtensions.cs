using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace RedLine.HealthChecks
{
    /// <summary>
    /// Provides extension methods for adding health checks.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds memory health checks.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddMemoryHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddWorkingSetHealthCheck(2L * 1024 * 1024 * 1024, "Working Set Memory should be below 2GB")
                .AddProcessAllocatedMemoryHealthCheck(1024 * 2, "Process allocated memory should be below 2GB")
                ;

            return services;
        }

        /// <summary>
        /// Adds URL health checks.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The health check name.</param>
        /// <param name="timeout">The timeout to receive a response.</param>
        /// <param name="uris">The uri(s) to check.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddUrlGroupHealthChecks(
            this IServiceCollection services,
            string name,
            TimeSpan timeout,
            params Uri[] uris)
        {
            services
                .AddHealthChecks()
                .AddUrlGroup(uris, name: name, timeout: timeout)
                ;

            return services;
        }

        /// <summary>
        /// Checks certificates for expiration.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The health check name.</param>
        /// <param name="timeout">The timeout to receive a response.</param>
        /// <param name="certPath">The path to the certificate file.</param>
        /// <param name="degraded">The threshold at which to report degraded.</param>
        /// <param name="unhealthy">The threshold at which to report unhealthy.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddCertificateExpirationHealthCheck(
            this IServiceCollection services,
            string name,
            TimeSpan timeout,
            string certPath,
            TimeSpan degraded,
            TimeSpan unhealthy)
        {
            services
                .AddHealthChecks()
                .AddCheck(name, cancellationToken =>
                {
                    using (var tokenWithTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                    {
                        tokenWithTimeout.CancelAfter(timeout);

                        using (var cert = new X509Certificate2(certPath))
                        {
                            var now = DateTime.UtcNow;
                            var expiration = DateTime.Parse(cert.GetExpirationDateString());
                            var degradedOn = expiration.Subtract(degraded);
                            var unhealthyOn = expiration.Subtract(unhealthy);

                            if (now >= unhealthyOn)
                            {
                                return HealthCheckResult.Unhealthy($"FAILED. Certificate expiration date: {expiration}.");
                            }
                            else if (now >= degradedOn)
                            {
                                return HealthCheckResult.Degraded($"DEGRADED. Certificate will be expiring in {(int)expiration.Subtract(now).TotalDays} days. ({expiration})");
                            }

                            return HealthCheckResult.Healthy($"OK. Certificate will not expire until {expiration}");
                        }
                    }
                });

            return services;
        }

        /// <summary>
        /// Checks for connectivity to Postgres.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The health check name.</param>
        /// <param name="timeout">The timeout to receive a response.</param>
        /// <param name="connectionString">The Postgres connection string.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddPostgresHealthCheck(
            this IServiceCollection services,
            string name,
            TimeSpan timeout,
            string connectionString)
        {
            services
                .AddHealthChecks()
                .AddNpgSql(connectionString, name: name, timeout: timeout);

            return services;
        }

        /// <summary>
        /// Checks for connectivity to RabbitMQ.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The health check name.</param>
        /// <param name="timeout">The timeout to receive a response.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRabbitMqHealthCheck(
            this IServiceCollection services,
            string name,
            TimeSpan timeout)
        {
            services
                .AddHealthChecks()
                .AddRabbitMQ(
                    container => container.GetRequiredService<IConnection>(),
                    name,
                    HealthStatus.Unhealthy,
                    timeout: timeout);

            return services;
        }
    }
}
