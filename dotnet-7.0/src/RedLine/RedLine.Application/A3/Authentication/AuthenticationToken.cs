using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using RedLine.Application.Exceptions;
using RedLine.Domain;

namespace RedLine.A3.Authentication
{
    /// <summary>
    /// Provides an implementation of the <see cref="IAuthenticationToken"/> interface.
    /// </summary>
    public class AuthenticationToken : IAuthenticationToken
    {
        /// <summary>
        /// Gets an <see cref="IAuthenticationToken"/> for an anonymous user.
        /// </summary>
        public static readonly IAuthenticationToken Anonymous = new AuthenticationToken();

        /// <summary>
        /// Initializes an anonymous authentication token.
        /// </summary>
        public AuthenticationToken()
        {
            Jwt = new JwtSecurityToken();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="jwt">The <see cref="JwtSecurityToken"/> for the current user.</param>
        internal AuthenticationToken(JwtSecurityToken jwt)
        {
            if (jwt == null)
            {
                throw new ArgumentNullException(nameof(jwt));
            }

            if (string.IsNullOrEmpty(jwt.Claims.FirstOrDefault(x => x.Type.Equals(AuthClaims.Upn))?.Value))
            {
                throw NotAuthenticatedException.BecauseMissingClaim(AuthClaims.Upn);
            }

            if (string.IsNullOrEmpty(jwt.Claims.FirstOrDefault(x => x.Type.Equals(AuthClaims.Name))?.Value))
            {
                throw NotAuthenticatedException.BecauseMissingClaim(AuthClaims.Name);
            }

            if (string.IsNullOrEmpty(jwt.Claims.FirstOrDefault(x => x.Type.Equals(AuthClaims.Email))?.Value))
            {
                throw NotAuthenticatedException.BecauseMissingClaim(AuthClaims.Email);
            }

            Jwt = jwt;
        }

        /// <inheritdoc/>
        public AuthenticationState AuthenticationState
        {
            get
            {
                var validFrom = Jwt.ValidFrom == default(DateTime) ? Jwt.ValidFrom : Jwt.ValidFrom.Subtract(ApplicationContext.AllowedClockSkew);
                var validTo = Jwt.ValidTo.Add(ApplicationContext.AllowedClockSkew);

                if (Jwt != null && DateTime.UtcNow >= validFrom && DateTime.UtcNow <= validTo)
                {
                    return AuthenticationState.Authenticated;
                }

                return AuthenticationState.Anonymous;
            }
        }

        /// <summary>
        /// Gets the JWT bearer token for the current user.
        /// </summary>
        public JwtSecurityToken Jwt { get; protected set; }

        /// <inheritdoc/>
        string IAuthenticationToken.Jwt
        {
            get => Jwt?.RawData;
            set => Jwt = string.IsNullOrEmpty(value) ? new JwtSecurityToken() : new JwtSecurityToken(value);
        }

        /// <inheritdoc/>
        public string FirstName => Jwt.Claims.FirstOrDefault(x => x.Type.Equals(AuthClaims.Given_Name))?.Value;

        /// <inheritdoc/>
        public string FullName => Jwt?.Claims.FirstOrDefault(x => x.Type.EndsWith(AuthClaims.Name))?.Value ?? "Anonymous";

        /// <inheritdoc/>
        public string LastName => Jwt?.Claims.FirstOrDefault(x => x.Type.EndsWith(AuthClaims.Family_Name))?.Value;

        /// <inheritdoc/>
        public string Login => Jwt?.Claims.FirstOrDefault(x => x.Type.EndsWith(AuthClaims.Email))?.Value;

        /// <inheritdoc/>
        public string UserId => Jwt?.Claims.FirstOrDefault(x => x.Type.EndsWith(AuthClaims.Upn))?.Value;

        /// <summary>
        /// Gets a value indicating whether the authentication will expire by the specified expiration date/time.
        /// </summary>
        /// <param name="expiration">The expiration date/time.</param>
        /// <returns>True if the token will expire by the specified expiration date/time; otherwise, false.</returns>
        public bool WillExpireBy(DateTime expiration)
        {
            return Jwt == null || expiration <= Jwt.ValidFrom || expiration >= Jwt.ValidTo;
        }
    }
}
