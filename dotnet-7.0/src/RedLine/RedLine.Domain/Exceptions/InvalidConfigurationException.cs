using System;
using System.Runtime.Serialization;

namespace RedLine.Domain.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a configuration setting is missing or invalid.
    /// </summary>
    [Serializable]
    public class InvalidConfigurationException : RedLineException
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">The status code for this exception.</param>
        /// <param name="setting">The setting that was missing or invalid.</param>
        /// <param name="message">A message explaining the exception.</param>
        /// <param name="cause">The exception that is the cause of this exception.</param>
        public InvalidConfigurationException(int statusCode, string setting, string message, Exception cause)
            : base(statusCode, message ?? $"The '{setting}' setting is missing or invalid.", cause)
        {
            Setting = setting;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">The status code for this exception.</param>
        /// <param name="setting">The setting that was missing or invalid.</param>
        /// <param name="cause">The exception that is the cause of this exception.</param>
        public InvalidConfigurationException(int statusCode, string setting, Exception cause)
            : this(statusCode, setting, null, cause)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">The status code for this exception.</param>
        /// <param name="setting">The setting that was missing or invalid.</param>
        /// <param name="message">A message explaining the exception.</param>
        public InvalidConfigurationException(int statusCode, string setting, string message)
            : this(statusCode, setting, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">The status code for this exception.</param>
        /// <param name="setting">The setting that was missing or invalid.</param>
        public InvalidConfigurationException(int statusCode, string setting)
            : this(statusCode, setting, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="setting">The setting that was missing or invalid.</param>
        /// <param name="message">A message explaining the exception.</param>
        /// <param name="cause">The exception that is the cause of this exception.</param>
        public InvalidConfigurationException(string setting, string message, Exception cause)
            : this(500, setting, message, cause)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="setting">The setting that was missing or invalid.</param>
        public InvalidConfigurationException(string setting)
            : this(setting, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="setting">The setting that was missing or invalid.</param>
        /// <param name="message">A message explaining the exception.</param>
        public InvalidConfigurationException(string setting, string message)
            : this(setting, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the config setting that was missing or invalid.
        /// </summary>
        public string Setting { get; protected set; }
    }
}
