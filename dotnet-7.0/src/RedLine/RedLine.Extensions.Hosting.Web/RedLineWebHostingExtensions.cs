using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using App.Metrics.Formatters.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using RedLine.A3.Authentication;
using RedLine.Application;
using RedLine.Data;
using RedLine.Domain;
using RedLine.Domain.Exceptions;

namespace RedLine.Extensions.Hosting.Web
{
    /// <summary>
    /// Extension methods for hosting a RedLine website or API.
    /// </summary>
    public static class RedLineWebHostingExtensions
    {
        /// <summary>
        /// Adds the RedLine web host services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRedLineWebHost(this IServiceCollection services)
        {
            return services
                .AddRabbitMQ()
                .AddRedLineDataServices()
                .AddRedLineApplicationServices()
                .AddJwtBearerAuthentication()
                .AddTenantId()
                .AddCorrelationId()
                .AddETag()
                .AddClientAddress()
                .AddMetrics()
                .AddMetricsEndpoints(options =>
                {
                    options.MetricsEndpointOutputFormatter = new MetricsPrometheusFormatter();
                    options.MetricsTextEndpointOutputFormatter = new MetricsJsonOutputFormatter();
                })
                .AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                })
                ;
        }

        /// <summary>
        /// Enables the RedLine web host middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRedLineWebHost(this IApplicationBuilder app)
        {
            return app
                .UseAuthenticationTokenMiddleware()
                .UseTenantIdMiddleware()
                .UseCorrelationIdMiddleware()
                .UseETagMiddleware()
                .UseClientAddressMiddleware()
                .UseMetricsEndpoint()
                .UseMetricsTextEndpoint()
                .UseRedLineHealthChecks()
                .UseApiVersioning()
                ;
        }

        /// <summary>
        /// Adds the RedLine <see cref="IAuthenticationToken"/> to the container and configures Bearer authentication.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services)
        {
            var publicKeyFile = ApplicationContext.AuthenticationServicePublicKeyPath;

            if (string.IsNullOrEmpty(publicKeyFile) || !File.Exists(publicKeyFile))
            {
                throw new InvalidConfigurationException(500, nameof(ApplicationContext.AuthenticationServicePublicKeyPath));
            }

            X509SecurityKey signingKey = null;

            try
            {
                signingKey = new X509SecurityKey(new X509Certificate2(publicKeyFile));
            }
            catch (Exception e)
            {
                throw new InvalidConfigurationException(500, nameof(ApplicationContext.AuthenticationServicePublicKeyPath), cause: e);
            }

            services
                .AddScoped<IAuthenticationToken, AuthenticationToken>()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var audience = ApplicationContext.AuthenticationServiceAudience;
                    var authority = ApplicationContext.AuthenticationServiceEndpoint;

                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.Audience = audience;
                    options.Authority = authority;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        IssuerSigningKey = signingKey,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuer = true,
                        ValidIssuer = authority,
                        ValidateLifetime = true,
                        ClockSkew = ApplicationContext.AllowedClockSkew,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = authFailed =>
                        {
                            var tenantId = authFailed.HttpContext.TenantId();
                            var correlationId = authFailed.HttpContext.CorrelationId();

                            var dictionary = new Dictionary<string, object>
                            {
                                { nameof(TenantId), tenantId },
                                { nameof(CorrelationId), correlationId },
                            };

                            var logger = authFailed
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<ILogger>();

                            using (logger.BeginScope(dictionary))
                            {
                                logger.LogWarning(
                                    authFailed.Exception,
                                    "[HTTP]==> JWT token validation FAILED!\n[HTTP]<== ERROR 500: {Message}",
                                    authFailed.Exception?.Message);
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = authSuccess =>
                        {
                            var logger = authSuccess
                                .HttpContext
                                .RequestServices
                                .GetRequiredService<ILogger>();

                            if (logger.IsEnabled(LogLevel.Debug))
                            {
                                var tenantId = authSuccess.HttpContext.TenantId();
                                var correlationId = authSuccess.HttpContext.CorrelationId();

                                var dictionary = new Dictionary<string, object>
                                {
                                    { nameof(TenantId), tenantId },
                                    { nameof(CorrelationId), correlationId },
                                };

                                using (logger.BeginScope(dictionary))
                                {
                                    logger.LogDebug("[HTTP]==> JWT token validation succeeded for {Name}.\n[HTTP]<== OK", authSuccess.Principal?.Identity?.Name ?? "<unknown>");
                                }
                            }

                            return Task.CompletedTask;
                        },
                    };
                });

            return services;
        }

        /// <summary>
        /// Enables the <see cref="IAuthenticationToken"/> middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        private static IApplicationBuilder UseAuthenticationTokenMiddleware(this IApplicationBuilder app)
        {
            return app
                .Use((httpContext, next) =>
                {
                    httpContext.RequestServices
                        .GetRequiredService<IAuthenticationToken>()
                        .Jwt = httpContext.AccessToken();

                    return next();
                });
        }

        /// <summary>
        /// Adds the RedLine <see cref="ITenantId"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddTenantId(this IServiceCollection services)
        {
            return services.AddScoped<ITenantId, TenantId>(_ => new TenantId());
        }

        /// <summary>
        /// Enables the <see cref="ITenantId"/> middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        private static IApplicationBuilder UseTenantIdMiddleware(this IApplicationBuilder app)
        {
            return app
                .Use((httpContext, next) =>
                {
                    httpContext.RequestServices
                        .GetRequiredService<ITenantId>()
                        .Value = httpContext.TenantId();

                    return next();
                });
        }

        /// <summary>
        /// Adds the RedLine <see cref="ICorrelationId"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddCorrelationId(this IServiceCollection services)
        {
            return services.AddScoped<ICorrelationId, CorrelationId>(_ => new CorrelationId());
        }

        /// <summary>
        /// Enables the <see cref="ICorrelationId"/> middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        private static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app)
        {
            return app
                .Use((httpContext, next) =>
                {
                    httpContext.RequestServices
                        .GetRequiredService<ICorrelationId>()
                        .Value = httpContext.CorrelationId();

                    return next();
                });
        }

        /// <summary>
        /// Adds the RedLine <see cref="IETag"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddETag(this IServiceCollection services)
        {
            return services.AddScoped<IETag, ETag>(_ => new ETag());
        }

        /// <summary>
        /// Enables the <see cref="IETag"/> middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        private static IApplicationBuilder UseETagMiddleware(this IApplicationBuilder app)
        {
            return app
                .Use(async (httpContext, next) =>
                {
                    httpContext.RequestServices
                        .GetRequiredService<IETag>()
                        .IncomingValue = httpContext.ETag();

                    httpContext.Response.OnStarting(() =>
                    {
                        var etag = httpContext.RequestServices
                            .GetRequiredService<IETag>()
                            .OutgoingValue;

                        if (!string.IsNullOrEmpty(etag))
                        {
                            httpContext.Response?.Headers.Add(HeaderNames.ETag, etag.Split(','));
                        }

                        return Task.CompletedTask;
                    });

                    await next().ConfigureAwait(false);
                });
        }

        /// <summary>
        /// Adds the RedLine <see cref="IClientAddress"/> services.
        /// </summary>
        /// <param name="services">The IoC container as an <see cref="IServiceCollection"/>.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddClientAddress(this IServiceCollection services)
        {
            return services.AddScoped<IClientAddress, ClientAddress>(_ => new ClientAddress());
        }

        /// <summary>
        /// Enables the <see cref="IClientAddress"/> middleware.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns><see cref="IApplicationBuilder"/>.</returns>
        private static IApplicationBuilder UseClientAddressMiddleware(this IApplicationBuilder app)
        {
            return app
                .Use((httpContext, next) =>
                {
                    httpContext.RequestServices
                        .GetRequiredService<IClientAddress>()
                        .Value = httpContext.RemoteIpAddress();

                    return next();
                });
        }
    }
}
