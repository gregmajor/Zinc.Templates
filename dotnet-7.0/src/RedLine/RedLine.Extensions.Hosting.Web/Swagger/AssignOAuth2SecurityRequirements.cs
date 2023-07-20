using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RedLine.Extensions.Hosting.Web.Swagger
{
    internal class AssignOAuth2SecurityRequirements : IOperationFilter
    {
        private readonly OpenApiSecurityScheme oAuth2SecurityScheme;

        public AssignOAuth2SecurityRequirements(OpenApiSecurityScheme oAuth2SecurityScheme)
        {
            this.oAuth2SecurityScheme = oAuth2SecurityScheme;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorizeAttribute = context.ApiDescription
                .ActionDescriptor
                .FilterDescriptors
                .Any(f => f.Filter is AuthorizeFilter);

            if (!hasAuthorizeAttribute)
            {
                return;
            }

            if (operation.Security == null)
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
            }

            var oauth2Requirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = oAuth2SecurityScheme.Name,
                            Type = ReferenceType.SecurityScheme,
                        },
                        UnresolvedReference = true,
                    },
                    new List<string>()
                },
            };

            operation.Security.Add(oauth2Requirement);
        }
    }
}