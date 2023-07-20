using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RedLine.Application.Exceptions;
using RedLine.Domain.Exceptions;

namespace Zinc.Templates.Host.Web
{
    /// <summary>
    /// An extension method to execute controller actions with standard error handling.
    /// </summary>
    internal static class ActionExecutor
    {
        internal static async Task<IActionResult> Execute(
            this Controller controller,
            ILogger logger,
            Func<Task<IActionResult>> action)
        {
            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (InvalidCommandOrQueryException e)
            {
                return controller.StatusCode(e.StatusCode, string.Join(' ', e.ValidationErrors));
            }
            catch (RedLineException e)
            {
                return controller.StatusCode(e.StatusCode, e.Message);
            }
            catch (NotImplementedException e)
            {
                return controller.StatusCode(501, e.Message);
            }
            catch (SecurityTokenExpiredException e)
            {
                logger?.LogError(
                    e,
                    "[HTTP]==> {Controller}/{Action}\n[HTTP]<== 401 UNAUTHORIZED: Security token expired.",
                    controller.ControllerContext.ActionDescriptor.ControllerName ?? string.Empty,
                    controller.ControllerContext.ActionDescriptor.ActionName ?? string.Empty);

                return controller.Unauthorized("Login has expired.");
            }
            catch (Exception e)
            {
                logger?.LogError(
                    e,
                    "[HTTP]==> {Controller}/{Action}\n[HTTP]<== 500 ERROR: {Message}",
                    controller.ControllerContext.ActionDescriptor.ControllerName ?? string.Empty,
                    controller.ControllerContext.ActionDescriptor.ActionName ?? string.Empty,
                    e.Message);

                return controller.StatusCode(500, "An unexpected exception occurred. Please try again.");
            }
        }
    }
}
