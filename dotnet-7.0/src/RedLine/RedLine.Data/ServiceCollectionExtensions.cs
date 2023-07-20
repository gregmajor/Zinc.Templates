using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Npgsql;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Data.Outbox;
using RedLine.Data.Outbox.Serialization;
using RedLine.Data.Repositories;
using RedLine.Data.Serialization;
using RedLine.Data.TypeHandlers;
using RedLine.Domain;
using RedLine.Domain.A3.Authorization.Repositories;
using RedLine.Domain.Repositories;

namespace RedLine.Data
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> used to register dependencies with the IoC container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// A custom <see cref="Dapper.SqlMapper.ITypeHandler"/> for <see cref="OutboxMessage"/>s.
        /// </summary>
        private static readonly OutboxMessageTypeHandler OutboxMessageTypeHandler = CreateOutboxMessageTypeHandler();

        /// <summary>
        /// Adds the data services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineDataServices(this IServiceCollection services)
        {
            ConfigureDapper();
            ConfigureJsonSerialization();

            return services
                .AddConnectionStrings()
                .AddScoped<IDbConnection, NpgsqlConnection>(BuildConnection)
                .AddScoped<IOutbox, Outbox.Outbox>()
                .AddScoped<IRepository, Repository>()
                .AddScoped<IRepository<Grant>, GrantRepository>()
                .AddScoped<IGrantRepository, GrantRepository>()
                ;
        }

        /// <summary>
        /// Adds the data services to the container strictly for migrations.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineMigrationServices(this IServiceCollection services)
        {
            ConfigureDapper();
            ConfigureJsonSerialization();

            return services
                .AddConnectionStrings()
                .AddScoped<IDbConnection, NpgsqlConnection>(BuildConnection);
        }

        private static IServiceCollection AddConnectionStrings(this IServiceCollection services)
        {
            /* NOTE:
             * This code searches for IConnectionString implementations and registers them in the container.
             * It registers not only the IConnectionString interface, but also the concrete type.
             * This allows injecting IEnumerable<IConnectionString>, or injecting a specific connection
             * string, such as PostgresConnectionString.
             * */
            return services.AddImplementations<IConnectionString>(
                typeof(AssemblyMarker).Assembly,
                ServiceLifetime.Scoped,
                true);
        }

        private static NpgsqlConnection BuildConnection(IServiceProvider container)
        {
            return new NpgsqlConnection(container.GetRequiredService<PostgresConnectionString>().Value);
        }

        private static void ConfigureDapper()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            SqlMapper.AddTypeHandler(typeof(OutboxMessage), OutboxMessageTypeHandler);
            SqlMapper.AddTypeHandler(typeof(IEnumerable<OutboxMessage>), OutboxMessageTypeHandler);
            SqlMapper.AddTypeHandler(new MoneyTypeHandler());
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Money>("public.redline_money");
        }

        private static OutboxMessageTypeHandler CreateOutboxMessageTypeHandler()
        {
            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Add(new OutboxMessageJsonConverter());

            return new OutboxMessageTypeHandler(serializerOptions);
        }

        private static void ConfigureJsonSerialization()
        {
            // Configure the default RedLine JSON serializer settings
            JsonConvert.DefaultSettings = () => RedLineNewtonsoftSerializerSettings.Default;
        }
    }
}
