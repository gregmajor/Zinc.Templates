using System;
using DOPA;
using DOPA.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Application.A3.Authorization;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    public class AuthorizationFixture
    {
        private readonly IServiceProvider container;

        public AuthorizationFixture()
        {
            var assembly = typeof(RedLine.Application.AssemblyMarker).Assembly;
            var wasmResourcePath = $"{assembly.GetName().Name}.A3.Authorization.authorization.wasm";
            using var stream = assembly.GetManifestResourceStream(wasmResourcePath);

            container = new ServiceCollection()
                .AddOpaPolicy<AuthorizationPolicy>(stream)
                .BuildServiceProvider();
        }

        internal IOpaPolicy<AuthorizationPolicy> CreatePolicy() => container.GetRequiredService<IOpaPolicy<AuthorizationPolicy>>();
    }
}
