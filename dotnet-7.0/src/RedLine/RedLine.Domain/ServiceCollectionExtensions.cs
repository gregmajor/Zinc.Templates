using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RedLine.Domain
{
    /// <summary>
    /// Provides assembly scanning registration methods for the <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds implementations of <typeparamref name="TInterface"/> to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <param name="assembly">The assembly to scan.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> to use fo the registration.</param>
        /// <param name="registerImplementingType">If true, both the interface and concrete implementation type will be registered.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        /// <remarks>IMPORTANT: Only public implementation types are supported.</remarks>
        /// <typeparam name="TInterface">The interface to add implementations.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is appropriate for this code.")]
        public static IServiceCollection AddImplementations<TInterface>(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime,
            bool registerImplementingType = false)
                => services.AddImplementations(assembly, typeof(TInterface), lifetime, registerImplementingType);

        /// <summary>
        /// Adds implementations of <paramref name="interfaceType"/> to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <param name="assembly">The assembly to scan.</param>
        /// <param name="interfaceType">The type of interface to add.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> to use fo the registration.</param>
        /// <param name="registerImplementingType">If true, both the interface and concrete implementation type will be registered.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        /// <remarks>IMPORTANT: Only public implementation types are supported.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is appropriate for this code.")]
        public static IServiceCollection AddImplementations(
            this IServiceCollection services,
            Assembly assembly,
            Type interfaceType,
            ServiceLifetime lifetime,
            bool registerImplementingType = false)
        {
            bool Predicate(Type i) => i.IsGenericType ? i.GetGenericTypeDefinition() == interfaceType : i == interfaceType;

            return services.AddImplementations(assembly, Predicate, lifetime, registerImplementingType);
        }

        private static IServiceCollection AddImplementations(
            this IServiceCollection services,
            Assembly assembly,
            Func<Type, bool> interfacePredicate,
            ServiceLifetime lifetime,
            bool registerImplementingType)
        {
            // This code searches for implementations of 'interfaceType' and registers them in the container.
            IEnumerable<(Type Interface, Type Implementation)> matches = from type in assembly.ExportedTypes
                where !type.IsAbstract && !type.IsGenericTypeDefinition
                let interfaces = type.GetInterfaces().Where(interfacePredicate)
                let matchingInterface = interfaces.FirstOrDefault()
                where matchingInterface != null
                select (matchingInterface, type);

            foreach (var (@interface, implementation) in matches)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Transient:

                        services.AddTransient(@interface, implementation);

                        if (registerImplementingType)
                        {
                            services.AddTransient(implementation, implementation);
                        }

                        break;
                    case ServiceLifetime.Scoped:

                        services.AddScoped(@interface, implementation);

                        if (registerImplementingType)
                        {
                            services.AddScoped(implementation, implementation);
                        }

                        break;
                    case ServiceLifetime.Singleton:

                        services.AddSingleton(@interface, implementation);

                        if (registerImplementingType)
                        {
                            services.AddSingleton(implementation, implementation);
                        }

                        break;
                    default:
                        throw new ArgumentException("The ServiceLifetime is invalid.", nameof(lifetime));
                }
            }

            return services;
        }
    }
}
