using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedLine.Application;
using RedLine.Data;
using RedLine.Domain;
using RedLine.Extensions.Hosting;
using RedLine.Extensions.Testing.Data;
using Serilog;
using Zinc.Templates.Application;
using Zinc.Templates.Data;
using Zinc.Templates.Data.Migrations;

namespace Zinc.Templates.FunctionalTests
{
    /// <summary>
    /// This class serves a similar role to the AspNetCore Startup class, in that it's an application bootstrapper.
    /// </summary>
    internal static class Startup
    {
        /// <summary>
        /// Configures the application services.
        /// </summary>
        /// <returns><see cref="IServiceProvider"/>.</returns>
        public static IServiceProvider ConfigureServices()
        {
            Environment.SetEnvironmentVariable("APP_ENTRYPOINT", typeof(Startup).Namespace);
            var configuration = new ConfigurationBuilder()
                .AddParameterStoreSettings(Environment.GetEnvironmentVariable("RL_APP_NAME"), "test")
                .Build();

            ApplicationContext.Init(configuration.GetApplicationContextConfiguration());

            var services = new ServiceCollection();

            Log.Logger = new LoggerConfiguration().CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger, true);

            services
                // things the Host normally sets up
                .AddSingleton<IConfiguration>(_ => configuration)
                .AddSingleton<ILoggerFactory>(new LoggerFactory())
                .AddTransient(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger("Default"))

                // Our services for testing
                .AddRedLineDataServices()
                .AddSingleton<NpgsqlTypeLoader>()
                .AddDataServices()
                .AddRedLineApplicationServices()
                .AddApplicationServices()

                // Miscellaneous required bits
                .AddFluentMigrator(configuration)
                .AddTransient<ICorrelationId, CorrelationId>()
                .AddTransient<IETag, ETag>()
                .AddTransient<ITenantId>(_ => new TenantId(WellKnownId.TestTenant))
                .AddTransient<IClientAddress, ClientAddress>(_ => new ClientAddress("127.0.0.1"))
                .AddTransient(_ => Mocks.AccessToken)
                .AddTransient(_ => Mocks.MessageSession)
                .AddTransient(_ => Mocks.Metrics)
                ;

            return services.BuildServiceProvider();
        }
    }
}
