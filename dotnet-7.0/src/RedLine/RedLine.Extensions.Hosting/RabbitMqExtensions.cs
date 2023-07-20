using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RedLine.Extensions.Hosting.RabbitMQ;

namespace RedLine.Extensions.Hosting
{
    /// <summary>
    /// Provides extension methods for RedLine hosts.
    /// </summary>
    public static class RabbitMqExtensions
    {
        /// <summary>
        /// Registers the RabbitMQ <see cref="IConnection"/> with the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            return services
                .AddSingleton<RabbitMqConnectionFactory>()
                .AddSingleton<IConnection>(container => container.GetRequiredService<RabbitMqConnectionFactory>().BuildConnection());
        }
    }
}
