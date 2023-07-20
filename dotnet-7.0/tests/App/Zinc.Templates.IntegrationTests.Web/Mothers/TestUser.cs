using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace Zinc.Templates.IntegrationTests.Web.Mothers
{
    /// <summary>
    /// Represents a test user used to generate jwt for authentication
    /// and set grants for authorization.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Login = {Login}, Id = {Id}")]
    public class TestUser
    {
        private List<Claim> claims = new();
        private Dictionary<string, object> grants = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestUser"/> class.
        /// </summary>
        public TestUser()
        {
        }

        public TestUser(string id, string login, string fullName)
        {
            this.Id = id;
            this.Login = login;
            this.FullName = fullName;
        }

        /// <summary>
        /// The Id of the user. The combination of the sub and issuer claims in a jwt token.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The user name used to log into the external system.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// The full name of the user.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        public IDictionary<string, object> Grants
        {
            get
            {
                if (grants?.Count == 0)
                {
                    return new Dictionary<string, object>
                    {
                        { "*:*:*", new { ExpiresOn = (long?)null } },
                    };
                }

                return grants;
            }
        }

        public static TestUser GetTestuser1() => new("00u1m72lw907Or1F9357", "testuser1@redline.com", "Test User 1");

        public static TestUser GetTestuser2() => new("00u1m75cg3kvQfDI8357", "testuser2@redline.com", "Test User 2");

        public static TestUser GetTestuser3() => new("00u1m75q8oWBZhbzv357", "testuser3@redline.com", "Test User 3");

        public Claim GetClaim(string claimName) => claims.Find(x => string.Equals(x.Type, claimName, StringComparison.CurrentCultureIgnoreCase));

        public bool HasClaims()
        {
            return claims.Count > 0;
        }

        public void SetClaims(List<Claim> claims)
        {
            this.claims = claims;
        }

        public ClaimsIdentity GetClaimsIdentity()
        {
            return new ClaimsIdentity(claims);
        }

        public TestUser WithGrant(string grant) => WithGrant(grant, null);

        public TestUser WithGrant(string grant, DateTime? expiresOn)
        {
            grants.Add(grant, new { ExpiresOn = expiresOn?.ToUniversalTime().Ticks });
            return this;
        }

        public TestUser WithoutGrants()
        {
            grants = null;
            return this;
        }
    }
}
