using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Application;
using RedLine.Domain;

namespace Zinc.Templates.Host.Web.Controllers
{
    /// <summary>
    /// Well-Known container to respond to well-known requests.
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route(".well-known")]
    public class WellKnownController : Controller
    {
        private readonly ILogger<WellKnownController> logger;
        private readonly IEnumerable<IActivity> activities;

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownController"/> class.
        /// </summary>
        /// <param name="logger">A diagnostic logger.</param>
        /// <param name="activities">A collection of all the <see cref="IActivity"/>s.</param>
        public WellKnownController(ILogger<WellKnownController> logger, IEnumerable<IActivity> activities)
        {
            this.logger = logger;
            this.activities = activities;
        }

        /// <summary>
        /// Returns list of scripts and styles required to use the micro-application web component.
        /// </summary>
        /// <param name="spa">
        ///     The name of the spa application. The resources are looked up from wwwroot folder using this name for the subfolder.
        ///     Value of "web" returns the resources for main subfolder.
        /// </param>
        /// <returns>A json object of the form `{scripts: string[], styles: string[]}`.</returns>
        [HttpGet("{spa}-component-resources")]
        public IActionResult ComponentResources(string spa)
        {
            var dirname = spa == "web" ? "zn-templates" : spa;
            var path = $"wwwroot/dist/{dirname}";

            try
            {
                var files = Directory.GetFiles(path);
                var result = new Dictionary<string, string[]>
                {
                    ["scripts"] = files.Where(p => p.EndsWith(".js") && Path.GetFileName(p).StartsWith("main")).Select(p => Path.GetRelativePath("wwwroot", p)).ToArray(),
                    ["styles"] = files.Where(p => p.EndsWith(".css")).Select(p => Path.GetRelativePath("wwwroot", p)).ToArray(),
                };
                return Json(result);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "{Error} finding well-known resource at {Path}: {Message}",
                    e.GetType().Name,
                    path,
                    e.Message);

                return StatusCode(500, $"Unable to load {spa}-component-resources.");
            }
        }

        /// <summary>
        /// Returns the application's well known information.
        /// </summary>
        /// <returns>A json object with the application's well known information.</returns>
        [HttpGet("application")]
        public IActionResult Application()
        {
            return Json(new
            {
                Name = ApplicationContext.ApplicationName,
                DisplayName = ApplicationContext.ApplicationDisplayName,
                Activities = this.activities
                    .Select(activity => new
                    {
                        Name = activity.ActivityName,
                        DisplayName = activity.ActivityDisplayName,
                        Description = activity.ActivityDescription,
                    })
                    .OrderBy(activity => activity.Name),
            });
        }
    }
}
