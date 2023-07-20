using System;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.IntegrationTests.Web.Mothers
{
    public static class GreetingMother
    {
        public static Greeting HelloThere() => new(Guid.NewGuid(), "Hello there", DateTimeOffset.UtcNow);

        public static Greeting HiThere() => new(Guid.NewGuid(), "Hi there", DateTimeOffset.UtcNow);
    }
}
