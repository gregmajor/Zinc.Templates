using System;
using System.Data;
using RedLine.A3;

namespace RedLine.Domain
{
    /// <summary>
    /// Provides extension methods for the <see cref="IActivityContext"/>.
    /// </summary>
    public static class ActivityContextExtensions
    {
        /// <summary>
        /// Gets the <see cref="IAccessToken"/> for the current user from the context.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The <see cref="IAccessToken"/> for the current user.</returns>
        public static IAccessToken AccessToken(this IActivityContext context)
        {
            return context.Get<IAccessToken>(nameof(AccessToken));
        }

        /// <summary>
        /// Gets the name of the current application from the context.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The name of the current application.</returns>
        public static string ApplicationName(this IActivityContext context)
        {
            return context.Get(nameof(ApplicationName), ApplicationContext.ApplicationName);
        }

        /// <summary>
        /// Gets the value of the <see cref="IClientAddress"/> from the context.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The value of the <see cref="IClientAddress"/>.</returns>
        public static string ClientAddress(this IActivityContext context)
        {
            return context.Get<IClientAddress>(nameof(ClientAddress))?.Value;
        }

        /// <summary>
        /// Gets the <see cref="IDbConnection"/> to the database.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The <see cref="IDbConnection"/> to the database.</returns>
        public static IDbConnection Connection(this IActivityContext context)
        {
            return context.Get<IDbConnection>(nameof(IDbConnection));
        }

        /// <summary>
        /// Gets the correlation identifier from the context.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The value of the correlation identifier.</returns>
        public static Guid CorrelationId(this IActivityContext context)
        {
            return context.Get<ICorrelationId>(nameof(CorrelationId))?.Value ?? Guid.Empty;
        }

        /// <summary>
        /// Gets the ETag value from the context that was originally provided by the client
        /// via the If-Match or If-None-Match HTTP headers. To get the "latest" ETag, usually
        /// after the repository does a read or save, use the LatestETag() extension method.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The value of the ETag.</returns>
        public static string ETag(this IActivityContext context)
        {
            return context.Get<IETag>(nameof(ETag))?.IncomingValue;
        }

        /// <summary>
        /// Sets the latest value of the ETag, usually after a repository read or save operation.
        /// This is the outgoing value that will be returned to the client.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <param name="newETag">The new ETag value.</param>
        public static void ETag(this IActivityContext context, string newETag)
        {
            var etag = context.Get<IETag>(nameof(ETag));

            if (etag != null)
            {
                etag.OutgoingValue = newETag;
            }
        }

        /// <summary>
        /// Gets the "latest" ETag value. If a repository set the outgoing value, either due to a read or save
        /// operation, that value will be returned; otherwise, the value provided by the client will be returned.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The value of the "latest" ETag.</returns>
        public static string LatestETag(this IActivityContext context)
        {
            var etag = context.Get<IETag>(nameof(ETag));

            if (etag != null)
            {
                return string.IsNullOrEmpty(etag.OutgoingValue)
                    ? etag.IncomingValue
                    : etag.OutgoingValue;
            }

            return null;
        }

        /// <summary>
        /// Gets a value from the <see cref="IActivityContext"/>.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <param name="key">The context item key.</param>
        /// <returns>The context item value as <typeparamref name="T"/>.</returns>
        public static T Get<T>(this IActivityContext context, string key) => context.Get(key, default(T));

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> from the context.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The <see cref="IServiceProvider"/> for the current request.</returns>
        public static IServiceProvider ServiceProvider(this IActivityContext context)
        {
            return context.Get<IServiceProvider>(nameof(IServiceProvider));
        }

        /// <summary>
        /// Gets the tenant identifier from the context.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/>.</param>
        /// <returns>The value of the tenant identifier.</returns>
        public static string TenantId(this IActivityContext context)
        {
            return context.Get<ITenantId>(nameof(TenantId))?.Value;
        }
    }
}
