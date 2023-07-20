using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using RedLine.Domain.Model;
using Zinc.Templates.Application.Commands.DeleteGreeting;
using Zinc.Templates.Application.Commands.PutGreeting;
using Zinc.Templates.Application.Queries.FindGreetings;
using Zinc.Templates.Application.Queries.GetGreeting;

namespace Zinc.Templates.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for aggregate operations.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/{tenantId}/greetings")]
    public class GreetingsApiController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<GreetingsApiController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">The command/query mediator we delegate requests to.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client as an http header.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="logger">The diagnostic logger to use.</param>
        public GreetingsApiController(IMediator mediator, ICorrelationId correlationId, ITenantId tenantId, ILogger<GreetingsApiController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Delete an aggregate.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="greetingId">The aggregate unique identifier.</param>
        /// <returns>204 NoContent on success.</returns>
        /// <response code="204">The aggregate was successfully deleted.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the validation errors.</response>
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
        [HttpDelete("{greetingId}", Name = nameof(DeleteGreeting))]
        public async Task<IActionResult> DeleteGreeting(string tenantId, Guid greetingId)
        {
            return await this.Execute(logger, async () =>
            {
                await mediator.Send(new DeleteGreetingCommand(tenantId, correlationId.Value, greetingId)).ConfigureAwait(false);

                return NoContent();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an aggregate by its identifier.
        /// </summary>
        /// <param name="greetingId">The aggregate unique identifier.</param>
        /// <returns>The greeting.</returns>
        /// <response code="200">The aggregate was found and returned in the response.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the validation errors.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="404">The aggregate was not found.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(GetGreetingResult), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("{greetingId}", Name = nameof(GetGreeting))]
        public async Task<IActionResult> GetGreeting(Guid greetingId)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new GetGreetingQuery(tenantId.Value, correlationId.Value, greetingId);

                var response = await mediator.Send(request).ConfigureAwait(false);

                if (response == null)
                {
                    return NotFound($"Greeting {greetingId} was not found.");
                }

                return Ok(response);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Upserts an aggregate.
        /// </summary>
        /// <param name="version">The api version to use.</param>
        /// <param name="model">The input model.</param>
        /// <returns>201 Created on success.</returns>
        /// <response code="201">The aggregate was upserted. The Location header will contain the uri to retrieve it.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the validation errors.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpPut]
        public async Task<IActionResult> PutGreeting(ApiVersion version, PutGreetingModel model)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new PutGreetingCommand(tenantId.Value, correlationId.Value, model.GreetingId, model.Message);

                var greetingId = await mediator.Send(request).ConfigureAwait(false);

                return CreatedAtAction(
                    nameof(GetGreeting),
                    "GreetingsApi",
                    new
                    {
                        version = version.ToString(),
                        tenantId = tenantId.Value,
                        greetingId = greetingId,
                    },
                    null);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Get greetings.
        /// </summary>
        /// <param name="searchPattern">Simple search pattern.</param>
        /// <returns>The greeting.</returns>
        /// <response code="200">The aggregate was found and returned in the response.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the validation errors.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="404">The aggregate was not found.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(PageableResult<FindGreetingsResult>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet(Name = nameof(FindGreetings))]
        public async Task<IActionResult> FindGreetings([FromQuery]string searchPattern)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new FindGreetingsQuery(tenantId.Value, correlationId.Value, searchPattern);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
