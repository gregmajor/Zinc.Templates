using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedLine.Extensions.Hosting;
using Serilog;

namespace Zinc.Templates.Data.Migrations
{
    internal static class Program
    {
        private static readonly string ApplicationHostName = typeof(Program).Namespace;

        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A return code, where 0 indicates success and any other value indicates an error.</returns>
        public static int Main(string[] args)
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
                    "[MIGRATIONS]==> Failed to initialize {Host}.\n[MIGRATIONS]<== ERROR 500: {Message}\n{Stack}",
                    ApplicationHostName,
                    e.Message,
                    e.StackTrace);

                return 1;
            }

            try
            {
                Log.Information("{Host} is starting...", ApplicationHostName);

                using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<Migrator>().RunMigrations();
                }

                Log.Information("{Host} has stopped.", ApplicationHostName);

                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(
                    e,
                    "[MIGRATIONS]==> A fatal {Error} occurred in {Host}.\n[MIGRATIONS]<== ERROR 500: {Message}\n{Stack}",
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

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddParameterStoreSettings())
                .ConfigureApplicationContext()
                .ConfigureSerilog(ApplicationHostName)
                .ConfigureServices((context, container) => container.AddFluentMigrator(context.Configuration))
            ;
    }
}
