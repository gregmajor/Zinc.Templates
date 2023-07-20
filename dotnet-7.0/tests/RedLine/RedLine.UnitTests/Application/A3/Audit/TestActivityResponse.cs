namespace RedLine.UnitTests.Application.A3.Audit
{
    internal class TestActivityResponse
    {
        public TestActivityResponse()
        {
            Message = "Ok";
        }

        public string Message { get; }

        public override string ToString()
        {
            return Message;
        }
    }
}
