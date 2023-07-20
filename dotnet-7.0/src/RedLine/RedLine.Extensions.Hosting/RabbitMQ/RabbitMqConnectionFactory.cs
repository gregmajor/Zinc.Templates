using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RedLine.Extensions.Hosting.RabbitMQ
{
    /// <summary>
    /// Factory for <see cref="IConnection"/>.
    /// </summary>
    internal class RabbitMqConnectionFactory
    {
        private readonly IConfiguration configuration;
        private IConnection connection;

        /// <summary>
        /// Creates a <see cref="RabbitMqConnectionFactory"/>.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public RabbitMqConnectionFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Builds a RabbitMq connection.
        /// </summary>
        /// <returns>A new RabbitMQ <see cref="IConnection"/>.</returns>
        public IConnection BuildConnection()
        {
            if (connection != null)
            {
                return connection;
            }

            var connectionString = new Data.RabbitMqConnectionString(configuration).Value;

            connection = BuildConnection(connectionString, true);

            return connection;
        }

        private IConnection BuildConnection(
            string connectionString,
            bool automaticRecoveryEnabled = true,
            bool useBackgroundThreadsForIO = true,
            bool disableRemoteCertificateValidation = false,
            bool useExternalAuthMechanism = false)
        {
            // Source: https://github.com/Particular/NServiceBus.RabbitMQ/blob/7610f482b685e16f4cf35b70d3b388642530b2f9/src/NServiceBus.Transport.RabbitMQ/Connection/ConnectionFactory.cs#L42
            var connectionConfiguration = RabbitMqConnectionConfiguration.Parse(connectionString);

            var connectionFactory = new ConnectionFactory
            {
                HostName = connectionConfiguration.Host,
                Port = connectionConfiguration.Port,
                VirtualHost = connectionConfiguration.VirtualHost,
                UserName = connectionConfiguration.UserName,
                Password = connectionConfiguration.Password,
                RequestedHeartbeat = TimeSpan.FromSeconds(connectionConfiguration.RequestedHeartbeat),
                NetworkRecoveryInterval = connectionConfiguration.RetryDelay,
                AutomaticRecoveryEnabled = automaticRecoveryEnabled,
                UseBackgroundThreadsForIO = useBackgroundThreadsForIO,
                Ssl =
                {
                    ServerName = connectionConfiguration.Host,
                    CertPath = connectionConfiguration.CertPath,
                    CertPassphrase = connectionConfiguration.CertPassphrase,
                    Version = SslProtocols.Tls12,
                    Enabled = connectionConfiguration.UseTls,
                },
            };

            if (disableRemoteCertificateValidation)
            {
                connectionFactory.Ssl.AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors |
                                                               SslPolicyErrors.RemoteCertificateNameMismatch |
                                                               SslPolicyErrors.RemoteCertificateNotAvailable;
            }

            if (useExternalAuthMechanism)
            {
                connectionFactory.AuthMechanisms = new List<IAuthMechanismFactory> { new ExternalMechanismFactory() };
            }

            return connectionFactory.CreateConnection();
        }
    }
}
