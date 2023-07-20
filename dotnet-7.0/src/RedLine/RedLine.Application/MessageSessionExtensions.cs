using System;
using System.Threading.Tasks;
using RedLine.Domain;

/* NOTE: We want the namespace here to be NServiceBus so that our extension methods
 * are easily found when using the IMessageSession in our code.
 * */
namespace NServiceBus
{
    /// <summary>
    /// Extension methods for the <see cref="IMessageSession"/> object.
    /// </summary>
    public static class MessageSessionExtensions
    {
        /// <summary>
        /// Published a message, and adds the correlationId and tenantId to the headers.
        /// </summary>
        /// <param name="messageSession">The <see cref="IMessageSession"/>.</param>
        /// <param name="message">The message to publish.</param>
        /// <param name="context">The <see cref="IActivityContext"/> for the current request.</param>
        /// <param name="configureOptions">Allows for additional <see cref="PublishOptions"/> configuration. This parameter is optional.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        public static async Task Publish(
            this IMessageSession messageSession,
            object message,
            IActivityContext context,
            Action<PublishOptions> configureOptions = null)
        {
            await Publish(
                messageSession,
                message,
                context.TenantId(),
                context.CorrelationId(),
                context.LatestETag(),
                configureOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Published a message, and adds the correlationId and tenantId to the headers.
        /// </summary>
        /// <param name="messageSession">The <see cref="IMessageSession"/>.</param>
        /// <param name="message">The message to publish.</param>
        /// <param name="tenantId">The current tenantId.</param>
        /// <param name="correlationId">The current correlationId.</param>
        /// <param name="etag">The ETag for the current request. This parameter is optional.</param>
        /// <param name="configureOptions">Allows for additional <see cref="PublishOptions"/> configuration. This parameter is optional.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        public static async Task Publish(
            this IMessageSession messageSession,
            object message,
            string tenantId,
            Guid correlationId,
            string etag = null,
            Action<PublishOptions> configureOptions = null)
        {
            var options = new PublishOptions();
            configureOptions?.Invoke(options);

            options.SetHeader(RedLineHeaderNames.TenantId, tenantId);
            options.SetHeader(RedLineHeaderNames.CorrelationId, correlationId.ToString());
            options.SetHeader(RedLineHeaderNames.ETag, etag ?? string.Empty);

            await messageSession.Publish(message, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the message to the specified endpoint and adds the correlationId and tenantId as headers.
        /// </summary>
        /// <param name="messageSession">The NServiceBus <see cref="IMessageSession"/>.</param>
        /// <param name="message">The message to publish.</param>
        /// <param name="endpoint">The endpoint to which the message will be sent.</param>
        /// <param name="context">The <see cref="IActivityContext"/> for the current request.</param>
        /// <param name="configureOptions">Allows for additional <see cref="SendOptions"/> configuration. This parameter is optional.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        public static async Task Send(
            this IMessageSession messageSession,
            object message,
            string endpoint,
            IActivityContext context,
            Action<SendOptions> configureOptions = null)
        {
            await Send(
                messageSession,
                message,
                endpoint,
                context.TenantId(),
                context.CorrelationId(),
                context.LatestETag(),
                configureOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the message to the specified endpoint and adds the correlationId and tenantId as headers.
        /// </summary>
        /// <param name="messageSession">The NServiceBus <see cref="IMessageSession"/>.</param>
        /// <param name="message">The message to publish.</param>
        /// <param name="endpoint">The endpoint to which the message will be sent.</param>
        /// <param name="tenantId">The current tenantId.</param>
        /// <param name="correlationId">The current correlationId.</param>
        /// <param name="etag">The ETag for the current request. This parameter is optional.</param>
        /// <param name="configureOptions">Allows for additional <see cref="SendOptions"/> configuration. This parameter is optional.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        public static async Task Send(
            this IMessageSession messageSession,
            object message,
            string endpoint,
            string tenantId,
            Guid correlationId,
            string etag = null,
            Action<SendOptions> configureOptions = null)
        {
            var options = new SendOptions();
            configureOptions?.Invoke(options);
            options.SetDestination(endpoint);

            options.SetHeader(RedLineHeaderNames.TenantId, tenantId);
            options.SetHeader(RedLineHeaderNames.CorrelationId, correlationId.ToString());
            options.SetHeader(RedLineHeaderNames.ETag, etag ?? string.Empty);

            await messageSession.Send(message, options).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the message to the local application's Messaging host.
        /// </summary>
        /// <param name="messageSession">The NServiceBus <see cref="IMessageSession"/>.</param>
        /// <param name="message">The message to publish.</param>
        /// <param name="context">The <see cref="IActivityContext"/> for the current request.</param>
        /// <param name="configureOptions">Allows for additional <see cref="SendOptions"/> configuration. This parameter is optional.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        /// <remarks>
        /// This method sets the SendOptions.RouteToThisEndpoint() setting, which will cause
        /// the message to be sent to the local application's messaging host.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        public static async Task SendToThisEndpoint(
            this IMessageSession messageSession,
            object message,
            IActivityContext context,
            Action<SendOptions> configureOptions = null)
        {
            await SendToThisEndpoint(
                messageSession,
                message,
                context.TenantId(),
                context.CorrelationId(),
                context.LatestETag(),
                configureOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends the message to the local application's Messaging host.
        /// </summary>
        /// <param name="messageSession">The NServiceBus <see cref="IMessageSession"/>.</param>
        /// <param name="message">The message to publish.</param>
        /// <param name="tenantId">The current tenantId.</param>
        /// <param name="correlationId">The current correlationId.</param>
        /// <param name="etag">The ETag for the current request. This parameter is optional.</param>
        /// <param name="configureOptions">Allows for additional <see cref="SendOptions"/> configuration. This parameter is optional.</param>
        /// <returns>A <see cref="Task"/> used to await the operation.</returns>
        /// <remarks>
        /// This method sets the SendOptions.RouteToThisEndpoint() setting, which will cause
        /// the message to be sent to the local application's messaging host.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        public static async Task SendToThisEndpoint(
            this IMessageSession messageSession,
            object message,
            string tenantId,
            Guid correlationId,
            string etag = null,
            Action<SendOptions> configureOptions = null)
        {
            var options = new SendOptions();
            configureOptions?.Invoke(options);
            options.RouteToThisEndpoint();

            options.SetHeader(RedLineHeaderNames.TenantId, tenantId);
            options.SetHeader(RedLineHeaderNames.CorrelationId, correlationId.ToString());
            options.SetHeader(RedLineHeaderNames.ETag, etag ?? string.Empty);

            await messageSession.Send(message, options).ConfigureAwait(false);
        }
    }
}
