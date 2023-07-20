using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using RedLine.Application.Exceptions;
using RedLine.Domain;

namespace RedLine.A3.Authentication
{
    /// <summary>
    /// Provides an implementation of the <see cref="IAuthenticationTokenProvider"/>.
    /// </summary>
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        private static readonly object SyncRoot = new object();
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
        private static readonly TimeSpan TokenExpirationThreshold = TimeSpan.FromMinutes(10);

        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="httpClient">An <see cref="HttpClient"/> used to call the authentication service.</param>
        /// <param name="cache">An <see cref="IMemoryCache"/> used to cache authorization data.</param>
        public AuthenticationTokenProvider(
            HttpClient httpClient,
            IMemoryCache cache)
        {
            this.httpClient = httpClient;
            this.cache = cache;
        }

        /// <inheritdoc/>
        public Task<IAuthenticationToken> GetServiceAuthenticationToken()
        {
            var authenticationToken = cache.Get<AuthenticationToken>(ApplicationContext.ServiceAccountName);

            if (authenticationToken == null || authenticationToken.WillExpireBy(DateTime.UtcNow.Add(TokenExpirationThreshold)))
            {
                /* NOTE:
                 * It is possible, especially in the Messaging host, that different threads could all reach this
                 * point at the same time and call into AuthN to get a new token. The lock helps mitigate that
                 * swarm of calls to AuthN. Note also that we are using the double-checked locking pattern here.
                 * https://en.wikipedia.org/wiki/Double-checked_locking
                */
                lock (SyncRoot)
                {
                    authenticationToken = cache.Get<AuthenticationToken>(ApplicationContext.ServiceAccountName);

                    if (authenticationToken == null || authenticationToken.WillExpireBy(DateTime.UtcNow.Add(TokenExpirationThreshold)))
                    {
                        var jwt = RequestToken()
                            .ConfigureAwait(false)
                            .GetAwaiter()
                            .GetResult();

                        authenticationToken = cache.Set(
                            ApplicationContext.ServiceAccountName,
                            new AuthenticationToken(jwt),
                            DateTimeOffset.UtcNow.Add(TokenLifetime));
                    }
                }
            }

            return Task.FromResult<IAuthenticationToken>(authenticationToken);
        }

        private async Task<JwtSecurityToken> RequestToken()
        {
            var disco = await httpClient.GetDiscoveryDocumentAsync(ApplicationContext.AuthenticationServiceEndpoint).ConfigureAwait(false);

            if (disco.IsError && (int)disco.HttpStatusCode >= 100 && (int)disco.HttpStatusCode <= 599)
            {
                throw new NotAuthenticatedException((int)disco.HttpStatusCode, disco.Error, disco.Exception);
            }

            if (disco.IsError)
            {
                throw new NotAuthenticatedException(500, $"{disco.Error}\t(No HTTP status returned)", disco.Exception);
            }

            var clientId = ApplicationContext.ServiceAccountName;
            var certPath = ApplicationContext.ServiceAccountPrivateKeyPath;
            var certPassword = ApplicationContext.ServiceAccountPrivateKeyPassword;

            string clientToken;

            using (var certificate = new X509Certificate2(certPath, certPassword))
            {
                clientToken = CreateClientToken(certificate, clientId, disco.TokenEndpoint);
            }

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                Scope = ApplicationContext.AuthenticationServiceAudience,
                ClientCredentialStyle = ClientCredentialStyle.PostBody,

                ClientAssertion =
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = clientToken,
                },
            };

            var response = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest).ConfigureAwait(false);

            if (response.IsError)
            {
                throw new NotAuthenticatedException((int)response.HttpStatusCode, response.Error, response.Exception);
            }

            return new JwtSecurityToken(response.AccessToken);
        }

        private string CreateClientToken(
            X509Certificate2 certificate,
            string clientId,
            string audience)
        {
            var now = DateTimeOffset.UtcNow;

            var token = new JwtSecurityToken(
                clientId,
                audience,
                new List<Claim>
                {
                    new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.Subject, clientId),
                    new Claim(JwtClaimTypes.IssuedAt, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                },
                now.UtcDateTime,
                now.Add(TokenLifetime).UtcDateTime,
                new SigningCredentials(new X509SecurityKey(certificate), SecurityAlgorithms.RsaSha256));

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
