using System.Collections.Generic;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public class ValidCultureNameTestCases : TestCases<string>
    {
        public override IEnumerable<string> GetTestCases()
        {
            yield return "en-US";
            yield return "en-GB";
            yield return "en-CA";
            yield return "fr-CA";
        }
    }
}
