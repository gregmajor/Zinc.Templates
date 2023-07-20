using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedLine.Data.Serialization;
using RedLine.Domain;
using RedLine.Domain.Exceptions;

namespace RedLine.A3.Audit
{
    /// <summary>
    /// Provides an implementation of the <see cref="IActivityAudited"/> event.
    /// </summary>
    /// <remarks>
    /// This class is a Decorator that executes an activity and combines details from the <typeparamref name="TRequest"/> and <typeparamref name="TResponse"/>.
    /// </remarks>
    /// <typeparam name="TRequest">The type of request being made.</typeparam>
    /// <typeparam name="TResponse">The type of response that is returned.</typeparam>
    public class ActivityAudit<TRequest, TResponse> : IActivityAudited
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/> providing details about the request.</param>
        /// <param name="request">The <typeparamref name="TRequest"/> to execute.</param>
        public ActivityAudit(IActivityContext context, TRequest request)
        {
            ActivityName = request.GetType().Name;
            ApplicationName = context.ApplicationName();
            ClientAddress = context.ClientAddress();
            CorrelationId = context.CorrelationId();
            Exception = null;
            Login = context.AccessToken().Login;
            Request = request;
            Response = default;
            StatusCode = 0;
            TenantId = context.TenantId();
            Timestamp = DateTime.UtcNow;
            UserId = context.AccessToken().UserId;
            UserName = context.AccessToken().FullName;
        }

        /// <inheritdoc/>
        public string ActivityName { get;  set; }

        /// <inheritdoc/>
        public string ApplicationName { get;  set; }

        /// <inheritdoc/>
        public string ClientAddress { get; set; }

        /// <inheritdoc/>
        public Guid CorrelationId { get;  set; }

        /// <summary>
        /// Gets the exception thrown while executing the activity, or null if no exception was thrown.
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <inheritdoc/>
        string IActivityAudited.Exception
        {
            get => Exception?.Message;
            set => _ = value;
        }

        /// <inheritdoc/>
        public string Login { get; set; }

        /// <summary>
        /// Gets the activity request object (command or query).
        /// </summary>
        public TRequest Request { get; protected set; }

        /// <inheritdoc/>
        string IActivityAudited.Request
        {
            get => JsonConvert.SerializeObject(Request, RedLineNewtonsoftSerializerSettings.IgnoreStream);
            set => _ = value;
        }

        /// <summary>
        /// Gets the activity response object.
        /// </summary>
        public TResponse Response { get; protected set; }

        /// <inheritdoc/>
        string IActivityAudited.Response
        {
            get => Response?.ToString();
            set => _ = value;
        }

        /// <inheritdoc/>
        public int StatusCode { get;  set; }

        /// <inheritdoc/>
        public string TenantId { get;  set; }

        /// <inheritdoc/>
        public DateTime Timestamp { get;  set; }

        /// <inheritdoc/>
        public string UserId { get;  set; }

        /// <inheritdoc/>
        public string UserName { get;  set; }

        /// <summary>
        /// Executes the activity and collects the resulting response or exception.
        /// </summary>
        /// <param name="activity">A callback function used to execute the activity.</param>
        /// <returns>The response from the activity execution.</returns>
        public async Task<TResponse> Decorate(Func<Task<TResponse>> activity)
        {
            try
            {
                Response = await activity().ConfigureAwait(false);

                StatusCode = 200;

                return Response;
            }
            catch (RedLineException e)
            {
                Exception = e;
                StatusCode = e.StatusCode;
                throw;
            }
            catch (Exception e)
            {
                Exception = e;
                StatusCode = 500;
                throw;
            }
            finally
            {
                Timestamp = DateTime.UtcNow;
            }
        }
    }
}
