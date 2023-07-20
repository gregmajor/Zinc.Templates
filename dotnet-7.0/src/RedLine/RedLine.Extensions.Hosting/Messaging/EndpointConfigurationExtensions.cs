using System;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using NServiceBus;
using RedLine.Extensions.Hosting.Messaging.Behaviors;

namespace RedLine.Extensions.Hosting.Messaging
{
    /// <summary>
    /// Provides some extension methods for an <see cref="EndpointConfiguration"/>.
    /// </summary>
    internal static class EndpointConfigurationExtensions
    {
        /// <summary>
        /// Prefix used when saving NServiceBus data in database.
        /// </summary>
        public static readonly string PersistenceTablePrefix = "messaging_";

        /// <summary>
        /// Configures the <see cref="EndpointConfiguration"/> as a send-only endpoint.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration"/> to configure.</param>
        /// <param name="transportConnectionString">The transport (RabbitMQ) connection string.</param>
        /// <param name="persistenceConnectionString">The persistence (Postgres) connection string.</param>
        /// <param name="criticalErrorHandler">The critical error handler to use.</param>
        /// <returns>The configured <see cref="EndpointConfiguration"/>.</returns>
        public static EndpointConfiguration ConfigureFullDuplexEndpoint(
            this EndpointConfiguration endpointConfiguration,
            string transportConnectionString,
            string persistenceConnectionString,
            Func<ICriticalErrorContext, Task> criticalErrorHandler)
        {
            endpointConfiguration
                .ConfigureDefaults(transportConnectionString, criticalErrorHandler)
                .ConfigurePersistence(persistenceConnectionString);

            // These behaviors apply to incoming messages. They set up the ITenantId and ICorrelationId, and provide a logging scope with the values.
            endpointConfiguration.Pipeline.Register(new IncomingRedLineHeadersBehavior(), "Parses the CorrelationId and TenantId headers from the incoming message and sets their values in the container.");
            endpointConfiguration.Pipeline.Register(new IncomingLoggingBehavior(), "Adds a logging scope with CorrelationId and TenantId to the pipeline.");

            return endpointConfiguration;
        }

        /// <summary>
        /// Configures the <see cref="EndpointConfiguration"/> as a send-only endpoint.
        /// </summary>
        /// <param name="endpointConfiguration">The <see cref="EndpointConfiguration"/> to configure.</param>
        /// <param name="transportConnectionString">The transport (RabbitMQ) connection string.</param>
        /// <param name="criticalErrorHandler">The critical error handler to use.</param>
        /// <returns>The configured <see cref="EndpointConfiguration"/>.</returns>
        public static EndpointConfiguration ConfigureSendOnlyEndpoint(
            this EndpointConfiguration endpointConfiguration,
            string transportConnectionString,
            Func<ICriticalErrorContext, Task> criticalErrorHandler)
        {
            endpointConfiguration
                .ConfigureDefaults(transportConnectionString, criticalErrorHandler)
                .SendOnly();

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigureDefaults(
            this EndpointConfiguration endpointConfiguration,
            string transportConnectionString,
            Func<ICriticalErrorContext, Task> criticalErrorHandler)
        {
            if (criticalErrorHandler != null)
            {
                endpointConfiguration.DefineCriticalErrorAction(criticalErrorHandler);
            }

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            endpointConfiguration
                .Conventions()
                .DefiningMessagesAs(MessageConventions.IsMessage)
                .DefiningCommandsAs(MessageConventions.IsCommand)
                .DefiningEventsAs(MessageConventions.IsEvent);

            endpointConfiguration
                .UseTransport<RabbitMQTransport>()
                .ConnectionString(transportConnectionString)
                .UseConventionalRoutingTopology();

            endpointConfiguration.Pipeline.Register(new OutgoingPublishFullTypeNameOnlyBehavior(), "Replaces the assembly qualified type name with the full Type name only.");

            return endpointConfiguration;
        }

        private static EndpointConfiguration ConfigurePersistence(
            this EndpointConfiguration endpointConfiguration,
            string persistenceConnectionString)
        {
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();

            persistence.TablePrefix(PersistenceTablePrefix);

            persistence.ConnectionBuilder(() => new NpgsqlConnection(persistenceConnectionString));

            var dialect = persistence.SqlDialect<SqlDialect.PostgreSql>();

            dialect.Schema("nsb");

            dialect.JsonBParameterModifier(
               modifier: parameter =>
               {
                   var parm = (NpgsqlParameter)parameter;
                   parm.NpgsqlDbType = NpgsqlDbType.Jsonb;
               });

            return endpointConfiguration;
        }
    }
}
