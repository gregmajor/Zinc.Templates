using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RedLine.Extensions.Hosting.Web.Swagger
{
    internal class ETagIfMatchRequirement : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if ("PUT".Equals(context.ApiDescription.HttpMethod, StringComparison.InvariantCultureIgnoreCase))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "If-Match",
                    In = ParameterLocation.Header,
                    Description = "Latest ETag from the resource.",
                    Required = false,
                });
            }
        }
    }
}