using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Data.Serialization;
using RedLine.Extensions.Hosting;
using RedLine.Extensions.Hosting.Web;
using Zinc.Templates.Application;
using Zinc.Templates.Data;

namespace Zinc.Templates.Host.Web
{
    internal class Startup
    {
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
                .AddRedLineWebHost()
                .AddRedLineHealthChecks(Configuration)
                .AddDataServices()
                .AddApplicationServices()
                .Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost)
                ;

            services
                .AddControllers(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddNewtonsoftJson(config => RedLineNewtonsoftSerializerSettings.ApplyDefaults(config.SerializerSettings));

            services.AddSwagger(Configuration, typeof(AssemblyMarker).Assembly);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The<see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseForwardedHeaders()
                .UseHsts()
                .UseStaticFiles()
                .UseSwaggerMiddleware(Configuration, typeof(AssemblyMarker).Assembly);

            app
                .UseRouting()
                .UseCors(policy =>
                {
                    var corsPolicy = Configuration
                        .GetSection(CorsPolicy.SectionName)
                        ?.Get<CorsPolicy>() ?? new CorsPolicy();

                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.WithOrigins(corsPolicy.AllowedOrigins.ToArray());
                    policy.WithExposedHeaders(corsPolicy.ExposedHeaders.Distinct().ToArray());

                    /* NOTE:
                     * The CORS spec says that AllowCredentials is ONLY valid when restricting origins.
                     * In other words, if you AllowAnyOrigin, then you cannot AllowCredentials.
                     */
                    if (corsPolicy.AllowedOrigins.Any(x => x != "*"))
                    {
                        policy.AllowCredentials();
                    }
                })
                .UseAuthentication()
                .UseAuthorization()
                .UseRedLineWebHost()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
