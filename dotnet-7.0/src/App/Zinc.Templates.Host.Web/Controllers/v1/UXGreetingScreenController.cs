using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using Zinc.Templates.Application.Queries.UXGreetingsScreen.UXGreeting;

namespace Zinc.Templates.Host.Web.Controllers.V1
{
    /// <summary>
    /// A UX controller for the Greeting screen. Note the route starts with 'ux' instead of 'api'.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}/greetings")]
    public class UXGreetingScreenController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<UXGreetingScreenController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">The command/query mediator we delegate requests to.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client as an http header.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="logger">The diagnostic logger to use.</param>
        public UXGreetingScreenController(IMediator mediator, ICorrelationId correlationId, ITenantId tenantId, ILogger<UXGreetingScreenController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Gets an aggregate by its identifier.
        /// </summary>
        /// <param name="greetingId">The aggregate unique identifier.</param>
        /// <returns>The ux model.</returns>
        /// <response code="200">The aggregate was found and returned in the response.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the validation errors.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="404">The aggregate was not found.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(UXGreetingResult), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("{greetingId}", Name = nameof(UXGreeting))]
        public async Task<IActionResult> UXGreeting(Guid greetingId)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXGreetingQuery(tenantId.Value, correlationId.Value, greetingId);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
