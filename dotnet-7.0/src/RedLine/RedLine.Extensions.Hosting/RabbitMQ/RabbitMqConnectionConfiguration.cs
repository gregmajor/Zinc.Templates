using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace RedLine.Extensions.Hosting.RabbitMQ
{
    // Original code came from NServiceBus RabbitMQ
    // https://github.com/Particular/NServiceBus.RabbitMQ/blob/7610f482b685e16f4cf35b70d3b388642530b2f9/src/NServiceBus.Transport.RabbitMQ/Connection/ConnectionFactory.cs

    /// <summary>
    /// Parses the RabbitMQ connection string.
    /// </summary>
    internal class RabbitMqConnectionConfiguration
    {
        private const bool DefaultUseTls = false;
        private const int DefaultPort = 5672;
        private const int DefaultTlsPort = 5671;
        private const string DefaultVirtualHost = "/";
        private const ushort DefaultRequestedHeartbeat = 60;
        private static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromSeconds(10);

        internal RabbitMqConnectionConfiguration(
            string host,
            int port,
            string virtualHost,
            string userName,
            string password,
            ushort requestedHeartbeat,
            TimeSpan retryDelay,
            bool useTls,
            string certPath,
            string certPassphrase)
        {
            Host = host;
            Port = port;
            VirtualHost = virtualHost;
            UserName = userName;
            Password = password;
            RequestedHeartbeat = requestedHeartbeat;
            RetryDelay = retryDelay;
            UseTls = useTls;
            CertPath = certPath;
            CertPassphrase = certPassphrase;
        }

        private delegate bool Convert<T>(string input, out T output);

        /// <summary>
        /// The RabbitMQ host.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// The port RabbitMQ is using.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// RabbitMQ virtual host.
        /// </summary>
        public string VirtualHost { get; }

        /// <summary>
        /// User name to connect to RabbitMQ.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Password to connect to RabbitMQ.
        /// </summary>
        public string Password { get; }

        public ushort RequestedHeartbeat { get; }

        public TimeSpan RetryDelay { get; }

        public bool UseTls { get; }

        public string CertPath { get; }

        public string CertPassphrase { get; }

        /// <summary>
        /// Parse a connection string into RabbitMQ connection configuration.
        /// </summary>
        /// <param name="connectionString">The connection string to parse.</param>
        /// <returns>A <see cref="RabbitMqConnectionConfiguration"/> object.</returns>
        public static RabbitMqConnectionConfiguration Parse(string connectionString)
        {
            var dictionary = new DbConnectionStringBuilder { ConnectionString = connectionString }
                .OfType<KeyValuePair<string, object>>()
                .ToDictionary(pair => pair.Key, pair => pair.Value.ToString(), StringComparer.OrdinalIgnoreCase);

            var invalidOptionsMessage = new StringBuilder();

            if (dictionary.ContainsKey("dequeuetimeout"))
            {
                invalidOptionsMessage.AppendLine("The 'DequeueTimeout' connection string option has been removed. Consult the documentation for further information.");
            }

            if (dictionary.ContainsKey("maxwaittimeforconfirms"))
            {
                invalidOptionsMessage.AppendLine("The 'MaxWaitTimeForConfirms' connection string option has been removed. Consult the documentation for further information.");
            }

            if (dictionary.ContainsKey("prefetchcount"))
            {
                invalidOptionsMessage.AppendLine("The 'PrefetchCount' connection string option has been removed. Use 'EndpointConfiguration.UseTransport<RabbitMQTransport>().PrefetchCount' instead.");
            }

            if (dictionary.ContainsKey("usepublisherconfirms"))
            {
                invalidOptionsMessage.AppendLine("The 'UsePublisherConfirms' connection string option has been removed. Use 'EndpointConfiguration.UseTransport<RabbitMQTransport>().UsePublisherConfirms' instead.");
            }

            var useTls = GetValue(dictionary, "useTls", bool.TryParse, DefaultUseTls, invalidOptionsMessage);
            var port = GetValue(dictionary, "port", int.TryParse, useTls ? DefaultTlsPort : DefaultPort, invalidOptionsMessage);
            var virtualHost = GetValue(dictionary, "virtualHost", DefaultVirtualHost);
            var userName = GetValue(dictionary, "userName", string.Empty);
            var password = GetValue(dictionary, "password", string.Empty);
            var requestedHeartbeat = GetValue(dictionary, "requestedHeartbeat", ushort.TryParse, DefaultRequestedHeartbeat, invalidOptionsMessage);
            var retryDelay = GetValue(dictionary, "retryDelay", TimeSpan.TryParse, DefaultRetryDelay, invalidOptionsMessage);
            var certPath = GetValue(dictionary, "certPath", string.Empty);
            var certPassPhrase = GetValue(dictionary, "certPassphrase", string.Empty);

            var host = default(string);

            if (dictionary.TryGetValue("host", out var value))
            {
                var hostsAndPorts = value.Split(',');

                if (hostsAndPorts.Length > 1)
                {
                    invalidOptionsMessage.AppendLine("Multiple hosts are no longer supported. If using RabbitMQ in a cluster, consider using a load balancer to represent the nodes as a single host.");
                }

                var parts = hostsAndPorts[0].Split(':');
                host = parts.ElementAt(0);

                if (host.Length == 0)
                {
                    invalidOptionsMessage.AppendLine("Empty host name in 'host' connection string option.");
                }

                if (parts.Length > 1 && !int.TryParse(parts[1], out port))
                {
                    invalidOptionsMessage.AppendLine($"'{parts[1]}' is not a valid Int32 value for the port in the 'host' connection string option.");
                }
            }
            else
            {
                invalidOptionsMessage.AppendLine("Invalid connection string. 'host' value must be supplied. e.g: \"host=myServer\"");
            }

            if (invalidOptionsMessage.Length > 0)
            {
                throw new NotSupportedException(invalidOptionsMessage.ToString().TrimEnd('\r', '\n'));
            }

            return new RabbitMqConnectionConfiguration(
                host,
                port,
                virtualHost,
                userName,
                password,
                requestedHeartbeat,
                retryDelay,
                useTls,
                certPath,
                certPassPhrase);
        }

        private static string GetValue(Dictionary<string, string> dictionary, string key, string defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        private static T GetValue<T>(Dictionary<string, string> dictionary, string key, Convert<T> convert, T defaultValue, StringBuilder invalidOptionsMessage)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                if (!convert(value, out defaultValue))
                {
                    invalidOptionsMessage.AppendLine($"'{value}' is not a valid {typeof(T).Name} value for the '{key}' connection string option.");
                }
            }

            return defaultValue;
        }
    }
}
