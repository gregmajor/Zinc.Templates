using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using RedLine.Domain;
using RedLine.Extensions.Hosting.Messaging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;

namespace RedLine.Extensions.Hosting
{
    /// <summary>
    /// Extension methods for the <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Initializes the <see cref="ApplicationContext"/> from config settings.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/>.</param>
        /// <returns><see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder ConfigureApplicationContext(this IHostBuilder builder)
        {
            return builder.ConfigureServices((context, _) =>
                ApplicationContext.Init(context.Configuration.GetApplicationContextConfiguration()));
        }

        /// <summary>
        /// Configures Serilog as the default logging framework.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="applicationHostName">The name of the application host.</param>
        /// <param name="additionalLogSinks">Additional log sinks, particularly useful for integration tests.</param>
        /// <returns><see cref="IHostBuilder"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Security Hotspot", "S4792:Configuring loggers is security-sensitive", Justification = "The logger's configuration is safe.")]
        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder, string applicationHostName, params ILogEventSink[] additionalLogSinks)
        {
            const string loggingLevelKey = "Serilog:MinimumLevel:Default";
            const string loggingLevelOverrideKey = "Serilog:MinimumLevel:Override";

            builder.ConfigureServices((context, container) =>
            {
                var loggerConfig = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ServiceName", applicationHostName)
                    .Enrich.WithMachineName();

                var loggingLevel = Enum.TryParse<LogEventLevel>(context.Configuration[loggingLevelKey], out var level)
                    ? level
                    : LogEventLevel.Debug;

                loggerConfig.MinimumLevel.Is(loggingLevel);

                var overrides = context.Configuration
                    .GetSection(loggingLevelOverrideKey)
                    ?.GetChildren()
                    .Select(setting => new KeyValuePair<string, LogEventLevel>(
                        setting.Key,
                        Enum.Parse<LogEventLevel>(setting.Value)))
                    ?? Enumerable.Empty<KeyValuePair<string, LogEventLevel>>();

                foreach (var @override in overrides)
                {
                    loggerConfig.MinimumLevel.Override(@override.Key, @override.Value);
                }

                if (ApplicationContext.Context.Equals("remote", StringComparison.InvariantCultureIgnoreCase))
                {
                    loggerConfig.WriteTo.Console(new JsonFormatter(renderMessage: true));
                }
                else
                {
                    loggerConfig.WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                        theme: AnsiConsoleTheme.Code);
                }

                foreach (var sink in additionalLogSinks ?? Array.Empty<ILogEventSink>())
                {
                    loggerConfig.WriteTo.Sink(sink);
                }

                Log.Logger = loggerConfig.CreateLogger();

                /* NOTE:
                 * This line adds a default ILogger to the container so it can be injected. Otherwise,
                 * only an ILogger<T> can be injected, at least that's what we ran across during testing.
                 * */
                container.AddSingleton(provider => provider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger(applicationHostName));
            });

            builder.UseSerilog();

            return builder;
        }

        /// <summary>
        /// Configures the NServiceBus endpoint for a host.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="endpointName">The NSB endpoint name for the application.</param>
        /// <param name="endpointType">The <see cref="EndpointType"/> to configure.</param>
        /// <param name="criticalErrorHandler">The error handler to invoke when a critical error occurs.</param>
        /// <returns><see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder ConfigureNServiceBus(
            this IHostBuilder builder,
            string endpointName,
            EndpointType endpointType,
            Func<ICriticalErrorContext, Task> criticalErrorHandler)
        {
            Func<HostBuilderContext, EndpointConfiguration> endpointConfigurationBuilder = context =>
            {
                var endpointConfiguration = new EndpointConfiguration(endpointName);
                var transportConnectionString = new Data.RabbitMqConnectionString(context.Configuration).Value;
                var persistenceConnectionString = new Data.PostgresConnectionString(context.Configuration).Value;

                if (endpointType == EndpointType.SendOnly)
                {
                    endpointConfiguration.ConfigureSendOnlyEndpoint(
                        transportConnectionString,
                        criticalErrorHandler ?? DefaultCriticalErrorHandler(endpointName));
                }
                else if (endpointType == EndpointType.FullDuplex)
                {
                    endpointConfiguration.ConfigureFullDuplexEndpoint(
                        transportConnectionString,
                        persistenceConnectionString,
                        criticalErrorHandler ?? DefaultCriticalErrorHandler(endpointName));
                }
                else
                {
                    throw new ArgumentException($"The {nameof(EndpointType)} argument is invalid.");
                }

                return endpointConfiguration;
            };

            return builder.UseNServiceBus(endpointConfigurationBuilder);
        }

        /// <summary>
        /// Configures the NServiceBus endpoint for a host.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="endpointName">The NSB endpoint name for the application.</param>
        /// <param name="endpointType">The <see cref="EndpointType"/> to configure.</param>
        /// <returns><see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder ConfigureNServiceBus(
            this IHostBuilder builder,
            string endpointName,
            EndpointType endpointType)
            => ConfigureNServiceBus(
                builder,
                endpointName,
                endpointType,
                null);

        private static Func<ICriticalErrorContext, Task> DefaultCriticalErrorHandler(string endpointName)
        {
            return async context =>
            {
                try
                {
                    Log.Fatal(
                        context.Exception,
                        "[NServiceBus]==> A fatal {Error} occurred in {Host}!\n[NServiceBus]<== ERROR 500: {Message}\n{Stack}",
                        context.Exception.GetType().Name,
                        endpointName,
                        context.Error,
                        context.Exception.StackTrace);

                    try
                    {
                        await context.Stop().ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(
                            e,
                            "[NServiceBus]==> An unhandled {Error} occurred while stopping the NServiceBus endpoint in {Host}!\n[NServiceBus]<== ERROR 500: {Message}\n{Stack}",
                            e.GetType().Name,
                            endpointName,
                            e.Message,
                            e.StackTrace);
                    }
                }
                finally
                {
                    Environment.FailFast(
                        $"[NServiceBus]==> A critical error has forced the host to shut down!\n[NServiceBus]<== ERROR: {context.Error}",
                        context.Exception);
                }
            };
        }
    }
}
