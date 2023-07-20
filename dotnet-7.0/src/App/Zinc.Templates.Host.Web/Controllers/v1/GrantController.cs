using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.A3.Authorization.Domain;
using RedLine.Application.Commands.Grants.AddGrant;
using RedLine.Application.Commands.Grants.RevokeGrant;
using RedLine.Domain;
using Zinc.Templates.Host.Web.Models.Authorization;

namespace Zinc.Templates.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for adding and revoking grants.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/{tenantId}")]
    public class GrantController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<GrantController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">The command/query mediator we delegate requests to.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client as an http header.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="logger">The diagnostic logger to use.</param>
        public GrantController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<GrantController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Bestows an activity grant to a user.
        /// </summary>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="userId">The user id of the grantee.</param>
        /// <param name="model">The <see cref="AddGrantModel"/> with additional params.</param>
        /// <returns>204 NoContent if the operation succeeded.</returns>
        /// <response code="204">The grant was successfully added.</response>
        /// <response code="400">The grant already exists, or a parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpPost("activities/{activityName}/grants/{userId}")]
        public async Task<IActionResult> AddActivityGrant(
            string activityName,
            string userId,
            AddGrantModel model)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new AddGrantCommand(
                    tenantId.Value,
                    correlationId.Value,
                    userId,
                    model.FullName,
                    GrantType.Activity,
                    activityName,
                    model.ExpiresOn);

                var response = await mediator.Send(request).ConfigureAwait(false);

                if (!response.Result)
                {
                    return BadRequest(response.AuditMessage);
                }

                return NoContent();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Revokes an activity grant from a user.
        /// </summary>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="userId">The user id of the grantee.</param>
        /// <returns>204 NoContent if the operation succeeded.</returns>
        /// <response code="204">The grant was successfully revoked.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the validation errors.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="404">The grant was not found.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpDelete("activities/{activityName}/grants/{userId}")]
        public async Task<IActionResult> RevokeActivityGrant(string activityName, string userId)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new RevokeGrantCommand(
                    tenantId.Value,
                    correlationId.Value,
                    userId,
                    GrantType.Activity,
                    activityName);

                var response = await mediator.Send(request).ConfigureAwait(false);

                if (!response.Result)
                {
                    return NotFound(response.AuditMessage);
                }

                return NoContent();
            }).ConfigureAwait(false);
        }
    }
}
