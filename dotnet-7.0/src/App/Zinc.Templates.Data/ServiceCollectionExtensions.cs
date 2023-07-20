using Microsoft.Extensions.DependencyInjection;
using RedLine.Domain;
using RedLine.Domain.Repositories;

namespace Zinc.Templates.Data
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
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            var assembly = typeof(AssemblyMarker).Assembly;

            // NOTE: This code will register any public IRepository<IAggregateRoot> implementations for you.
            services
                .AddImplementations(
                    assembly,
                    typeof(IRepository<>),
                    ServiceLifetime.Scoped);
            return services;
        }
    }
}
