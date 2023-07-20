using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedLine.Domain;
using RedLine.Extensions.Hosting;
using RedLine.Extensions.Testing.Data;
using Serilog.Core;
using Zinc.Templates.Data.Migrations;

namespace Zinc.Templates.IntegrationTests.Web
{
    internal static class Startup
    {
        private static IHostBuilder CreateHostBuilder(params ILogEventSink[] additionalLogSinks)
        {
            Environment.SetEnvironmentVariable("APP_ENTRYPOINT", typeof(Startup).Namespace);
            var hostName = typeof(Startup).Namespace;

            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null)
                .ConfigureAppConfiguration(builder => builder.AddParameterStoreSettings(null, "test"))
                .ConfigureApplicationContext()
                .ConfigureSerilog(hostName, additionalLogSinks)
                .ConfigureNServiceBus(hostName, EndpointType.SendOnly)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.AllowSynchronousIO = true;
                    });
                    webBuilder.UseStartup<Host.Web.Startup>();
                    webBuilder.ConfigureTestServices(services =>
                    {
                        /*
                        * Need to replace some services with mocks or stubs? Here's the place to do it.
                        */
                    });
                });
        }

        public static IHostBuilder CreateTestHostBuilder(params ILogEventSink[] additionalLogSinks)
        {
            return CreateHostBuilder(additionalLogSinks)
                .ConfigureServices(services =>
                {
                    services
                        .AddScoped<NpgsqlTypeLoader>()
                        .AddTransient<ITenantId>(_ => new TenantId(WellKnownIds.TenantId));
                });
        }

        public static IServiceProvider CreateServiceProviderForMigrations()
        {
            return CreateHostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddFluentMigrator(context.Configuration);
                })
                .Build()
                .Services;
        }
    }
}
