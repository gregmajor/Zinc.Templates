using System.Collections.Generic;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public class InvalidCultureNameTestCases : TestCases<string>
    {
        public override IEnumerable<string> GetTestCases()
        {
            yield return string.Empty;
            yield return " ";
            yield return "Zork";
            yield return null;
        }
    }
}
