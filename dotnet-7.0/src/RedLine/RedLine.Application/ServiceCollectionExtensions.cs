using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using DOPA.DependencyInjection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RedLine.A3;
using RedLine.A3.Authentication;
using RedLine.A3.Authorization;
using RedLine.Application.A3.Authorization;
using RedLine.Application.A3.Authorization.PolicyData;
using RedLine.Application.Behaviors;
using RedLine.Domain;
using RedLine.Domain.Exceptions;

namespace RedLine.Application
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
        public static IServiceCollection AddRedLineApplicationServices(this IServiceCollection services)
        {
            // Configure the various libraries we use.
            services
                .AddMediatRServices()
                .AddFluentValidation<AssemblyMarker>()
                .AddAutoMapper(typeof(AssemblyMarker))
                .AddMemoryCache()
                .AddStackExchangeRedisCache(config => config.Configuration = ApplicationContext.RedisServiceEndpoint)
                ;

            // Configure the RedLine services.
            services
                .AddAuthentication()
                .AddAuthorization()
                .AddActivities<AssemblyMarker>()
                .AddScoped<IActivityContext, ActivityContext>()
                .AddTransient<IAccessToken, AccessToken>()
                ;

            return services;
        }

        /// <summary>
        /// Adds fluent validators in the assembly containing the specified type to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        /// <typeparam name="T">The type whose assembly should be scanned for fluent validators.</typeparam>
        public static IServiceCollection AddFluentValidation<T>(this IServiceCollection services)
        {
            /* NOTE:
             * The AssemblyScanner we use below will find the validators. However, it only finds them if the
             * validator classes are public. That sucks because ideally we should make the validators internal
             * classes. We "could" write our own assembly scanner, but for now just use what we have.
             */
            FluentValidation.AssemblyScanner
                .FindValidatorsInAssemblyContaining<T>()
                .ForEach(validator => services.AddTransient(validator.InterfaceType, validator.ValidatorType));

            return services;
        }

        /// <summary>
        /// Adds activities in the assembly containing the specified type to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        /// <typeparam name="T">The type whose assembly should be scanned for activities.</typeparam>
        public static IServiceCollection AddActivities<T>(this IServiceCollection services)
        {
            typeof(T).Assembly
                .GetExportedTypes()
                .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition && type.IsAssignableTo(typeof(IActivity)))
                .Select(type => FormatterServices.GetUninitializedObject(type) as IActivity)
                .ForEach(a => services.AddSingleton(a));

            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            try
            {
                services.AddHttpClient<IAuthenticationTokenProvider, AuthenticationTokenProvider>(client =>
                {
                    client.BaseAddress = new Uri(ApplicationContext.AuthenticationServiceEndpoint);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
            }
            catch (UriFormatException e)
            {
                throw new InvalidConfigurationException(nameof(ApplicationContext.AuthenticationServiceEndpoint), null, e);
            }

            return services;
        }

        private static IServiceCollection AddAuthorization(this IServiceCollection services)
        {
            var assembly = typeof(AssemblyMarker).Assembly;
            var wasmResourcePath = $"{assembly.GetName().Name}.A3.Authorization.authorization.wasm";
            using var stream = assembly.GetManifestResourceStream(wasmResourcePath);

            services
                .AddOpaPolicy<AuthorizationPolicy>(stream)
                .AddSingleton<Activities>()
                .AddTransient<ActivityGroups>()
                .AddTransient<Grants>()
                .AddScoped<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>()
                .AddScoped<IAuthorizationPolicy>(container =>
                {
                    return container
                        .GetRequiredService<IAuthorizationPolicyProvider>()
                        .GetPolicy()
                        .GetAwaiter()
                        .GetResult();
                })
                .AddHttpClient<IActivityGroupService, ActivityGroupService>(client =>
                {
                    client.BaseAddress = new Uri(ApplicationContext.AuthorizationServiceEndpoint);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });

            return services;
        }

        private static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            /* NOTE:
             * The order of these pipeline components is important:
             *   1. LoggingBehavior so it can log everything, successes and errors.
             *   2. MetricsBehavior because we collect metrics about both the errors and successes that occur.
             *   3. AuditBehavior because we audit everything, successes and errors.
             *   4. TransactionBehavior to ensure we only use one connection.
             *   5. AuthorizationBehavior because if a user is not authorized for the request, we need to stop
             *      the request and let the error bubble up to all the other behaviors.
             *   6. ValidationBehavior so we can stop the request if there are validation issues. This comes after
             *      the authorization behavior to avoid inadvertent data disclosure in validation messages.
             */

            services
                .AddMediatR(typeof(AssemblyMarker))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                ;

            return services;
        }
    }
}
