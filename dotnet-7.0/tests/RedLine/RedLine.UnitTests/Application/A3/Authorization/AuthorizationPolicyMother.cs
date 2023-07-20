using System;
using DOPA;
using RedLine.Application.A3.Authorization;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    public static class AuthorizationPolicyMother
    {
        internal static AuthorizationPolicy Policy(this IOpaPolicy<AuthorizationPolicy> opaPolicy, Func<OpaPolicyDataBuilder, OpaPolicyDataBuilder> dataCustomizations)
        {
            var data = dataCustomizations(new OpaPolicyDataBuilder()).BuildData();
            opaPolicy.SetData(data);
            return new(opaPolicy, AuthorizationTestBase.TenantId, "local@redline.services");
        }
    }
}
