namespace RedLine.UnitTests.Application.Validation
{
    public class MyOptionsWithRuleSets
    {
        public bool RunAllRuleSets { get; set; } = false;

        public bool RunDefaultRuleSet { get; set; } = false;

        public bool FailTheTestDefault { get; set; } = false;

        public bool RunRuleSet1 { get; set; } = false;

        public bool FailTheTestRuleSet1 { get; set; } = false;

        public bool RunRuleSet2 { get; set; } = false;

        public bool FailTheTestRuleSet2 { get; set; } = false;
    }
}
