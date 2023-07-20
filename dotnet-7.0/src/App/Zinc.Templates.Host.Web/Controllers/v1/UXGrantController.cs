using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Application.Queries.UXGetGrantableActivities;
using RedLine.Domain;

namespace Zinc.Templates.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for managing grants from a UI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}")]
    public class UXGrantController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<UXGrantController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">The command/query mediator we delegate requests to.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client as an http header.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="logger">The diagnostic logger to use.</param>
        public UXGrantController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<UXGrantController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Bestows an activity grant to a user.
        /// </summary>
        /// <param name="userId">The user id of the grantee.</param>
        /// <returns>The collection of grantable activities for the user.</returns>
        /// <response code="200">The grantable activities were returned.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(IEnumerable<UXGrantableActivity>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("activities/grants/{userId}")]
        public async Task<IActionResult> UXGetGrantableActivities(string userId)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXGetGrantableActivitiesQuery(
                    tenantId.Value,
                    correlationId.Value,
                    userId);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
