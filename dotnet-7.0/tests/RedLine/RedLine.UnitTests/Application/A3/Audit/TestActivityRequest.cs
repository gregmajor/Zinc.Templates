using System;
using RedLine.Application.Commands;

namespace RedLine.UnitTests.Application.A3.Audit
{
    internal class TestActivityRequest : CommandBase<TestActivityResponse>
    {
        public TestActivityRequest()
            : base("FakeTenant", Guid.NewGuid())
        {
        }

        public override string ActivityDescription => throw new NotImplementedException();

        public override string ActivityDisplayName => throw new NotImplementedException();
    }
}
