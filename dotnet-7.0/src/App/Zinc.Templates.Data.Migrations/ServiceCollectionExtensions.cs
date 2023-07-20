using System;
using System.Net.Http.Headers;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.Application.A3.Authorization;
using RedLine.Data;
using RedLine.Domain;
using RedLine.Domain.Exceptions;

namespace Zinc.Templates.Data.Migrations
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> used to register dependencies with the IoC container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the FluentMigrator services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Security Hotspot", "S4792:Configuring loggers is security-sensitive", Justification = "The logger's configuration is safe.")]
        public static IServiceCollection AddFluentMigrator(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddRedLineMigrationServices()
                .AddActivityGroupService()
                .AddFluentMigratorCore()
                .ConfigureRunner(runner => runner
                    .AddPostgres()
                    .WithGlobalConnectionString(_ => new PostgresConnectionString(configuration).Value)
                    .ScanIn(new[]
                    {
                        typeof(AssemblyMarker).Assembly,
                        typeof(RedLine.Data.AssemblyMarker).Assembly,
                    }).For.EmbeddedResources()
                    .ScanIn(new[]
                    {
                        typeof(AssemblyMarker).Assembly,
                        typeof(RedLine.Data.AssemblyMarker).Assembly,
                    }).For.Migrations())
                .Configure<RunnerOptions>(opt => opt.Tags = GetMigrationTags())
                .AddLogging(builder => builder.AddFluentMigratorConsole())
                .AddScoped<Migrator>();

            return services;
        }

        private static IServiceCollection AddActivityGroupService(this IServiceCollection services)
        {
            services
                .AddCorrelationId()
                .AddAuthentication()
                .AddJwtServiceAuthentication()
                .AddMemoryCache()
                .AddStackExchangeRedisCache(config => config.Configuration = ApplicationContext.RedisServiceEndpoint)
                .AddHttpClient<IActivityGroupService, ActivityGroupService>(client =>
                {
                    client.BaseAddress = new Uri(ApplicationContext.AuthorizationServiceEndpoint);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });

            return services;
        }

        private static string[] GetMigrationTags()
        {
            // In case the DOTNET_ENVIRONMENT is not set, use ASPNETCORE_ENVIRONMENT.
            string envName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Entry assembly tag allows us to run migrations based on how the app has started running.
            // The value will be the namespace of the entry point.
            string entryPoint = Environment.GetEnvironmentVariable("APP_ENTRYPOINT");
            return new[] { envName, entryPoint };
        }

        private static IServiceCollection AddCorrelationId(this IServiceCollection services)
        {
            return services.AddScoped<ICorrelationId, CorrelationId>(_ => new CorrelationId());
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            try
            {
                services.AddHttpClient<IAuthenticationTokenProvider, AuthenticationTokenProvider>(client =>
                {
                    client.BaseAddress = new Uri(ApplicationContext.AuthenticationServiceEndpoint);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
            }
            catch (UriFormatException e)
            {
                throw new InvalidConfigurationException(nameof(ApplicationContext.AuthenticationServiceEndpoint), null, e);
            }

            return services;
        }

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
    }
}
