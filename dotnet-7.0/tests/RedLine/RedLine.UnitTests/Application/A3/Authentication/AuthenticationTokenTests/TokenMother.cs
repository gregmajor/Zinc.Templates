using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RedLine.A3.Authentication;

namespace RedLine.UnitTests.Application.A3.Authentication.AuthenticationTokenTests
{
    public static class TokenMother
    {
        public static JwtSecurityToken CreateJwt(string userId, string userName, string login) => CreateJwt(userId, userName, login, DateTime.Now.AddHours(4));

        public static JwtSecurityToken CreateJwt(string userId, string userName, string login, DateTime expires)
        {
            var claims = new List<Claim>();

            AddClaim(claims, AuthClaims.Upn, userId);
            AddClaim(claims, AuthClaims.Name, userName);
            AddClaim(claims, AuthClaims.Email, login);

            return new JwtSecurityToken(claims: claims, expires: expires);
        }

        public static JwtSecurityToken CreateJwt(string userId, string userName, string login, string firstName, string lastName, DateTime expires)
        {
            var claims = new List<Claim>();

            AddClaim(claims, AuthClaims.Upn, userId);
            AddClaim(claims, AuthClaims.Name, userName);
            AddClaim(claims, AuthClaims.Email, login);
            AddClaim(claims, AuthClaims.Given_Name, firstName);
            AddClaim(claims, AuthClaims.Family_Name, lastName);

            return new JwtSecurityToken(claims: claims, expires: expires);
        }

        public static AuthenticationToken CreateToken(string userId, string userName, string login) => new AuthenticationToken(CreateJwt(userId, userName, login));

        private static void AddClaim(List<Claim> claims, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                claims.Add(new Claim(name, value));
            }
        }
    }
}
