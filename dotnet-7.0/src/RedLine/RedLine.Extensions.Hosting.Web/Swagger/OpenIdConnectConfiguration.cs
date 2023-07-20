using System;
using System.Diagnostics.CodeAnalysis;

namespace RedLine.Extensions.Hosting.Web.Swagger
{
    [SuppressMessage("Naming Rules", "SA1300", Justification = "Deserialized data structure")]
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable IDE1006 // Naming Styles
    internal class OpenIdConnectConfiguration
    {
#pragma warning disable S1144 // Unused private types or members should be removed
        public Uri issuer { get; set; }

        public Uri jwks_uri { get; set; }

        public Uri authorization_endpoint { get; set; }

        public Uri token_endpoint { get; set; }

        public Uri userinfo_endpoint { get; set; }

        public Uri end_session_endpoint { get; set; }

        public Uri check_session_iframe { get; set; }

        public Uri revocation_endpoint { get; set; }

        public Uri introspection_endpoint { get; set; }

        public Uri device_authorization_endpoint { get; set; }

        public bool frontchannel_logout_supported { get; set; }

        public bool frontchannel_logout_session_supported { get; set; }

        public bool backchannel_logout_supported { get; set; }

        public bool backchannel_logout_session_supported { get; set; }

        public string[] scopes_supported { get; set; }

        public string[] claims_supported { get; set; }

        public string[] grant_types_supported { get; set; }

        public string[] response_types_supported { get; set; }

        public string[] response_modes_supported { get; set; }

        public string[] token_endpoint_auth_methods_supported { get; set; }

        public string[] subject_types_supported { get; set; }

        public string[] id_token_signing_alg_values_supported { get; set; }

        public string[] code_challenge_methods_supported { get; set; }
#pragma warning restore S1144 // Unused private types or members should be removed
    }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore S3459 // Unassigned members should be removed
}