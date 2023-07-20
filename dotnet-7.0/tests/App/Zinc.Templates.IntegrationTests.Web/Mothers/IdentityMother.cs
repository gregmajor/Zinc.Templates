using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using RedLine.A3.Authentication;
using RedLine.Domain;

namespace Zinc.Templates.IntegrationTests.Web.Mothers
{
    public static class IdentityMother
    {
        public static readonly string ClientId = "exampletenant-shell";
        public static readonly string IdentityProvider = "exampletenant.okta.com";
        public static readonly string ApiClaim = "redline.app";

        public static string Issuer => ApplicationContext.AuthenticationServiceEndpoint;

        public static string Upn(TestUser user) => $"{user.Id}@{IdentityProvider}";

        public static void AddDefaultClaimsToUser(TestUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(AuthClaims.Name, user.FullName),
                new Claim(AuthClaims.Email, user.Login),
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.Issuer, Issuer),
                new Claim(JwtClaimTypes.ClientId, ClientId),
                new Claim(JwtClaimTypes.IdentityProvider, IdentityProvider),
                new Claim(JwtClaimTypes.Scope, ApiClaim),
                new Claim(JwtClaimTypes.Scope, "openid"),
                new Claim(JwtClaimTypes.Scope, "profile"),
                new Claim("upn", Upn(user)),
            };
            user.SetClaims(claims);
        }

        public static SecurityTokenDescriptor GetDefaultSecurityTokenDescriptor(TestUser user)
        {
            if (!user.HasClaims())
            {
                AddDefaultClaimsToUser(user);
            }

            var subject = user.GetClaimsIdentity();
            var now = DateTime.UtcNow;
            return new SecurityTokenDescriptor
            {
                Audience = ApiClaim,
                Expires = now.AddMinutes(Convert.ToInt32(10)),
                Issuer = Issuer,
                Subject = subject,
            };
        }

        public static string GenerateJWTToken(TestUser user)
        {
            var securityTokenDescriptor = GetDefaultSecurityTokenDescriptor(user);
            var certPath = Environment.GetEnvironmentVariable("Authentication__SigningCert__Path");
            X509Certificate2 signingCert = new(certPath, Environment.GetEnvironmentVariable("Authentication__SigningCert__Password"));
            X509SecurityKey privateKey = new(signingCert);
            var tokenHandler = new JwtSecurityTokenHandler();

            securityTokenDescriptor.SigningCredentials =
                new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256Signature);

            JwtSecurityToken stoken = (JwtSecurityToken)tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(stoken);
        }
    }
}
