using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RedLine.Domain;
using RedLine.Extensions.Hosting;
using Serilog;

namespace Zinc.Templates.Host.Jobs
{
    internal static class Program
    {
        private static readonly string ApplicationHostName = typeof(Program).Namespace;

        public static async Task<int> Main(string[] args)
        {
            IHost host = null;

            try
            {
                host = CreateHostBuilder(args).Build();
            }
            catch (Exception e)
            {
                Log.Fatal(
                    e,
                    "[JOBS]==> Failed to initialize {Host}.\n[JOBS]<== ERROR 500: {Message}\n{Stack}",
                    ApplicationHostName,
                    e.Message,
                    e.StackTrace);

                return 1;
            }

            try
            {
                Log.Information("{Host} is starting...", ApplicationHostName);

                await host.RunAsync().ConfigureAwait(false);

                Log.Information("{Host} has stopped.", ApplicationHostName);

                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(
                    e,
                    "[JOBS]==> A fatal {Error} occurred in {Host}.\n[JOBS]<== ERROR 500: {Message}\n{Stack}",
                    e.GetType().Name,
                    ApplicationHostName,
                    e.Message,
                    e.StackTrace);

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // NOTE: The order in which these calls are strung together is important!
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddParameterStoreSettings())
                .ConfigureApplicationContext()
                .ConfigureSerilog(ApplicationHostName)
                .ConfigureNServiceBus(ApplicationContext.ApplicationName, EndpointType.SendOnly)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => options.AllowSynchronousIO = true);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
