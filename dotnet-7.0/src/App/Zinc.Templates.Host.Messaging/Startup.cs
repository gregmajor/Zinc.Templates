using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Extensions.Hosting;
using RedLine.Extensions.Hosting.Messaging;
using Zinc.Templates.Application;
using Zinc.Templates.Data;

namespace Zinc.Templates.Host.Messaging
{
    internal class Startup
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRedLineMessagingHost()
                .AddRedLineHealthChecks(Configuration)
                .AddDataServices()
                .AddApplicationServices()
                ;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseRedLineMessagingHost();
        }
    }
}
