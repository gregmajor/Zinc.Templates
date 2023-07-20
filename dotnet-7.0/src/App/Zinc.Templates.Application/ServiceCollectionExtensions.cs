using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Application;

namespace Zinc.Templates.Application
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> used to register dependencies with the IoC container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the application services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddMediatR(typeof(AssemblyMarker))
                .AddFluentValidation<AssemblyMarker>()
                .AddAutoMapper(typeof(AssemblyMarker))
                .AddActivities<AssemblyMarker>()
                ;
            return services;
        }
    }
}
