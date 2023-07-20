using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using MediatR.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using RedLine.Extensions.Hosting.Web.Swagger;

namespace RedLine.Extensions.Hosting.Web
{
    /// <summary>
    /// Extension methods Swagger.
    /// </summary>
    public static class SwaggerExtensions
    {
        private static IDictionary<Type, ControllerData> controllerDataCache = null;

        private static IDictionary<string, OpenApiInfo> swaggerDocInfo = null;

        /// <summary>
        /// Registers Swagger.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            var swaggerConfig = SwaggerConfiguration.GetSwaggerConfiguration(configuration);

            if (swaggerConfig.Disabled)
            {
                return services;
            }

            BuildSwaggerCache(assembly);

            var httpClient = new HttpClient();

            var oidcConfiguration = GetConfiguration(httpClient, swaggerConfig.OAuth2Authority)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            services
                .AddSwaggerGen(options =>
                {
                    foreach (var info in swaggerDocInfo)
                    {
                        options.SwaggerDoc(info.Key, info.Value);
                    }

                    options.IncludeXmlComments(System.IO.Path.Combine(AppContext.BaseDirectory, assembly.GetName().Name + ".xml"), true);
                    options.UseInlineDefinitionsForEnums();

                    var oauth2Scheme = BuildOAuth2SecurityScheme(oidcConfiguration, swaggerConfig);
                    options.AddSecurityDefinition(oauth2Scheme.Name, oauth2Scheme);
                    options.OperationFilter<AssignOAuth2SecurityRequirements>(oauth2Scheme);
                    options.OperationFilter<ETagIfMatchRequirement>();
                });

            return services;
        }

        /// <summary>
        /// Configures the Swagger endpoints.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app, IConfiguration configuration, Assembly assembly)
        {
            var swaggerConfig = SwaggerConfiguration.GetSwaggerConfiguration(configuration);

            if (swaggerConfig.Disabled)
            {
                return app;
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var swaggerDoc in swaggerDocInfo.Keys)
                {
                    var info = SwaggerExtensions.swaggerDocInfo[swaggerDoc];
                    var url = $"{swaggerDoc}/swagger.json";
                    var name = $"{info.Title} API v{info.Version}";
                    options.SwaggerEndpoint(url, name);
                }

                options.OAuthClientId(swaggerConfig.ClientId);
                options.OAuthAppName(swaggerConfig.ClientName);
                options.OAuth2RedirectUrl(GetOAuth2RedirectUrl(swaggerConfig).ToString());

                app.MapWhen(
                    context =>
                        {
                            var path = context.Request.Path.Value;
                            return path.Equals("/swagger-oauth2/oauth2-redirect.html", StringComparison.OrdinalIgnoreCase);
                        },
                    config => config.UseStaticFiles(
                        new StaticFileOptions
                        {
                            FileProvider = new ManifestEmbeddedFileProvider(assembly),
                        }));
            });

            return app;
        }

        private static void BuildSwaggerCache(Assembly assembly)
        {
            var controllerData = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsOpenGeneric() && t.GetCustomAttribute<ApiControllerAttribute>() != null)
                .Select(t => new KeyValuePair<Type, ControllerData>(t, new ControllerData(t)));

            controllerDataCache = new ConcurrentDictionary<Type, ControllerData>(controllerData);

            swaggerDocInfo = new Dictionary<string, OpenApiInfo>();

            foreach (var groupName in GroupNames())
            {
                var info = new OpenApiInfo
                {
                    Title = $"{groupName} Api",
                    Version = groupName.Version().ToString(),
                };

                if (groupName.Deprecated())
                {
                    info.Title = $"{info.Title} (Deprecated)";
                }

                swaggerDocInfo.Add(groupName, info);
            }
        }

        private static async Task<OpenIdConnectConfiguration> GetConfiguration(HttpClient httpClient, string authority)
        {
            var configurationUrl = GetWellKnownConfigurationUrl(authority);

            var response = await httpClient.GetAsync(configurationUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<OpenIdConnectConfiguration>().ConfigureAwait(false);

            return content;
        }

        private static OpenApiSecurityScheme BuildOAuth2SecurityScheme(OpenIdConnectConfiguration oidcConfig, SwaggerConfiguration authConfig)
        {
            return new OpenApiSecurityScheme
            {
                Name = $"OAuth {authConfig.OAuth2Authority}",
                Type = SecuritySchemeType.OAuth2,
                In = ParameterLocation.Header,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        TokenUrl = oidcConfig.token_endpoint,
                        AuthorizationUrl = oidcConfig.authorization_endpoint,
                        Scopes = new Dictionary<string, string>
                        {
                            { "redline.app", "redline.app" },
                        },
                    },
                },
                Description = $"OAuth {authConfig.OAuth2Authority}",
            };
        }

        private static Uri GetOAuth2RedirectUrl(SwaggerConfiguration config)
        {
            var oauth2RedirectUrl = new UriBuilder(config.OAuth2RedirectUrl)
            {
                Path = "swagger-oauth2/oauth2-redirect.html",
            };

            return oauth2RedirectUrl.Uri;
        }

        private static Uri GetWellKnownConfigurationUrl(string authority)
        {
            var openIdConnectUrl = new UriBuilder(authority)
            {
                Path = ".well-known/openid-configuration",
            };
            return openIdConnectUrl.Uri;
        }

        private static string ControllerName(this Type controllerType)
        {
            return controllerType.Name.EndsWith("Controller")
                ? controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length)
                : controllerType.Name;
        }

        private static bool Deprecated(this string groupName)
        {
            return controllerDataCache.Values
                .Where(cd => cd.GroupName.Equals(groupName))
                .Any(cd => cd.Deprecated);
        }

        private static bool Deprecated(this Type controllerType)
        {
            var versionAttribute = controllerType.GetCustomAttribute<ApiVersionAttribute>()
                                   ?? new ApiVersionAttribute("1.0");

            return versionAttribute.Deprecated;
        }

        private static string GroupName(this Type controllerType)
        {
            var apiExplorerSettings = controllerType.GetCustomAttribute<ApiExplorerSettingsAttribute>()
                ?? new ApiExplorerSettingsAttribute { GroupName = controllerType.ControllerName() };

            return apiExplorerSettings.GroupName;
        }

        private static IEnumerable<string> GroupNames()
        {
            return controllerDataCache.Values.Select(cd => cd.GroupName).Distinct();
        }

        private static ApiVersion Version(this string groupName)
        {
            return controllerDataCache.Values
                .Where(cd => cd.GroupName.Equals(groupName))
                .SelectMany(cd => cd.Versions)
                .Max();
        }

        private static IEnumerable<ApiVersion> Versions(this Type controllerType)
        {
            var versionAttribute = controllerType.GetCustomAttribute<ApiVersionAttribute>()
                ?? new ApiVersionAttribute("1.0");

            return versionAttribute.Versions;
        }

        private sealed class ControllerData
        {
            public ControllerData(Type controllerType)
            {
                GroupName = controllerType.GroupName();
                Deprecated = controllerType.Deprecated();
                Versions = controllerType.Versions();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "This class is private.")]
            public string GroupName { get; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "This class is private.")]
            public bool Deprecated { get; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "This class is private.")]
            public IEnumerable<ApiVersion> Versions { get; }
        }
    }
}
