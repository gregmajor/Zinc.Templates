using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RedLine.Domain
{
    /// <summary>
    /// Provides contextual information about the running application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The ApplicationContext is here to share common properties needed among the different layers
    /// of the RedLine code. It is NOT meant to be a dumping ground for application properties.
    /// </para>
    /// <para>
    /// However, these properties can and should be used by your application as needed. One useful feature
    /// of this class is that it holds ALL environment variables and config file settings. If you know the
    /// name of what you're after, you can call ApplicationContext.Get(name). One benefit of doing this is
    /// that any environment variable placeholders (e.g. %SOME_VARIABLE%) will be automatically expanded
    /// for you. If, for some reason, you just want to Expand(anArbitraryString), you can also call the
    /// ApplicationContext.Expand(value) method wherever you like.
    /// </para>
    /// <para>
    /// So, while this class it not intended as a place to put settings in your application, it can certainly
    /// be used by your application to provide handy functionality.
    /// </para>
    /// <para>
    /// For example, suppose you have a custom configuration section. You might try something like this:
    /// <code>
    /// public class MyConfig
    /// {
    ///     public string MySetting => ApplicationContext.Get($"{nameof(MyConfig)}:{nameof(MySetting)}");
    /// }
    /// </code>
    /// Ok, I made that up and have no idea if it would actually work, but it's worth a try!
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "An exclamation mark is valid punctuation, damn it!")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Violates sort ordering.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2339:Public constant members should not be used", Justification = "We need a constant value here for attributes.")]
    public static class ApplicationContext
    {
        private static readonly Regex ExpansionPattern = new Regex("%([^%]+)%", RegexOptions.IgnoreCase);
        private static IDictionary<string, string> context = new Dictionary<string, string>();

        /// <summary>
        /// Gets the application 'proper' name (e.g. Argon.Canary).
        /// </summary>
        public const string ApplicationName = "Zinc.Templates";

        /// <summary>
        /// Gets the application 'system' name (e.g. ar-canary).
        /// </summary>
        public const string ApplicationSystemName = "zn-templates";

        /// <summary>
        /// Gets the application 'display' name (e.g. Canary).
        /// </summary>
        public static string ApplicationDisplayName => Get(nameof(ApplicationDisplayName));

        /// <summary>
        /// Gets the 'audience' for the authentication token.
        /// </summary>
        public static string AuthenticationServiceAudience => Get(nameof(AuthenticationServiceAudience));

        /// <summary>
        /// Gets the authentication service endpoint, or authority.
        /// </summary>
        public static string AuthenticationServiceEndpoint => Get(nameof(AuthenticationServiceEndpoint));

        /// <summary>
        /// Gets the authentication service public key path.
        /// </summary>
        public static string AuthenticationServicePublicKeyPath => Get(nameof(AuthenticationServicePublicKeyPath));

        /// <summary>
        /// Gets the authorization service endpoint.
        /// </summary>
        public static string AuthorizationServiceEndpoint => Get(nameof(AuthorizationServiceEndpoint));

        /// <summary>
        /// Gets the context in which the application is running (local, cicd, remote, etc).
        /// </summary>
        public static string Context => Environment.GetEnvironmentVariable("RL_APP_CONTEXT") ?? "local";

        /// <summary>
        /// Gets the Postgres host.
        /// </summary>
        public static string PostgresHost => Get(nameof(PostgresHost));

        /// <summary>
        /// Gets the Postgres user password.
        /// </summary>
        public static string PostgresPassword => Get(nameof(PostgresPassword));

        /// <summary>
        /// Gets the Postgres user name.
        /// </summary>
        public static string PostgresUser => Get(nameof(PostgresUser));

        /// <summary>
        /// Gets the RabbitMQ host name.
        /// </summary>
        public static string RabbitMqHost => Get(nameof(RabbitMqHost));

        /// <summary>
        /// Gets the RabbitMQ user password.
        /// </summary>
        public static string RabbitMqPassword => Get(nameof(RabbitMqPassword));

        /// <summary>
        /// Gets the RabbitMQ user name.
        /// </summary>
        public static string RabbitMqUser => Get(nameof(RabbitMqUser));

        /// <summary>
        /// Gets the RabbitMQ virtual host name.
        /// </summary>
        public static string RabbitMqVHost => Get(nameof(RabbitMqVHost));

        /// <summary>
        /// Gets the redis host name.
        /// </summary>
        public static string RedisServiceEndpoint => Get(nameof(RedisServiceEndpoint));

        /// <summary>
        /// Gets the application service account name.
        /// </summary>
        public static string ServiceAccountName => Get(nameof(ServiceAccountName));

        /// <summary>
        /// Gets the application service account private key path.
        /// </summary>
        public static string ServiceAccountPrivateKeyPath => Get(nameof(ServiceAccountPrivateKeyPath));

        /// <summary>
        /// Gets the application service account private key path.
        /// </summary>
        public static string ServiceAccountPrivateKeyPassword => Get(nameof(ServiceAccountPrivateKeyPassword));

        /// <summary>
        /// Clock skew allowed for checking authentication tokens.
        /// Configuration value is expected to be in a format valid for TimeSpan.Parse().
        /// </summary>
        public static TimeSpan AllowedClockSkew => TimeSpan.Parse(Get(nameof(AllowedClockSkew), "00:05:00"));

        /// <summary>
        /// Initializes the context with its variables.
        /// </summary>
        /// <param name="context">The context variables.</param>
        public static void Init(IDictionary<string, string> context)
        {
            ApplicationContext.context = new Dictionary<string, string>(context, StringComparer.OrdinalIgnoreCase);
            ApplicationContext.context[nameof(ApplicationName)] = ApplicationName;
            ApplicationContext.context[nameof(ApplicationSystemName)] = ApplicationSystemName;
        }

        /// <summary>
        /// Expands the environment variables in the specified string.
        /// </summary>
        /// <param name="value">The string value with environment variable placeholders.</param>
        /// <returns>The string with any environment variables expanded, or the value if it is null or empty string.</returns>
        public static string Expand(string value)
        {
            return ExpandInternal(value, new List<string>());
        }

        /// <summary>
        /// Get a value from the application context.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>Expanded value corresponding to the key. Throws <see cref="Exceptions.InvalidConfigurationException" /> if not found.</returns>
        public static string Get(string key)
        {
            return GetInternal(key, new List<string>()) ?? throw new Exceptions.InvalidConfigurationException(key);
        }

        /// <summary>
        /// Get a value from application context, but use defaultValue if not found.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="defaultValue">Default value to use, in case the key doesn't exist.</param>
        /// <returns>The value for the key or the default value.</returns>
        public static string Get(string key, string defaultValue)
        {
            return GetInternal(key, new List<string>()) ?? defaultValue;
        }

        private static string GetInternal(string key, IList<string> searchPath)
        {
            EnsureNoCircularReferenceExists(key, searchPath);

            searchPath.Add(key);

            string result = context.TryGetValue(key, out var value)
                ? ExpandInternal(value, searchPath)
                : null;

            searchPath.RemoveAt(searchPath.Count - 1);

            return result;
        }

        private static void EnsureNoCircularReferenceExists(string key, IList<string> searchPath)
        {
            if (searchPath.Contains(key))
            {
                throw new Exceptions.InvalidConfigurationException(key, $"Circular reference detected in configuration path {string.Join(" > ", searchPath)}");
            }
        }

        private static string ExpandInternal(string value, IList<string> searchPath)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return ExpansionPattern.Replace(
                Environment.ExpandEnvironmentVariables(value),
                match => GetInternal(match.Groups[1].Value, searchPath));
        }
    }
}
