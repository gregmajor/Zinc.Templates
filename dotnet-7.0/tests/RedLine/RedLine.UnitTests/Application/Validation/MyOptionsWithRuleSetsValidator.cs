using System.Collections.Generic;
using FluentValidation;
using RedLine.Application;

namespace RedLine.UnitTests.Application.Validation
{
    public class MyOptionsWithRuleSetsValidator :
        AbstractValidator<MyOptionsWithRuleSets>,
        IDetermineValidationRuleSetByConfig<MyOptionsWithRuleSets>
    {
        // ruleSet magic strings per FluentValidation docs; https://docs.fluentvalidation.net/en/latest/rulesets.html
        public static readonly string RuleSetDefault = "default";
        public static readonly string[] AllRuleSets = new string[] { "*" };

        public static readonly string RuleSet1 = "RuleSet1";
        public static readonly string RuleSet2 = "RuleSet2";

        public MyOptionsWithRuleSetsValidator()
        {
            // this is put in a magical ruleSet named "default"
            RuleFor(x => x.FailTheTestDefault).Must(x => x.Equals(false));

            RuleSet(RuleSet1, () => RuleFor(x => x.FailTheTestRuleSet1).Must(x => x.Equals(false)));

            RuleSet(RuleSet2, () => RuleFor(x => x.FailTheTestRuleSet2).Must(x => x.Equals(false)));
        }

        public string[] WhichRuleSets(MyOptionsWithRuleSets config)
        {
            if (config.RunAllRuleSets)
            {
                return AllRuleSets;
            }

            var runMe = new List<string>();
            if (config.RunDefaultRuleSet)
            {
                runMe.Add(RuleSetDefault);
            }

            if (config.RunRuleSet1)
            {
                runMe.Add(RuleSet1);
            }

            if (config.RunRuleSet2)
            {
                runMe.Add(RuleSet2);
            }

            // if we return null, then our validator will behave as if no
            // rule set has been chosen and the default will run
            // if we return any rule set name, then we need to return 'default'
            //  as well if we intend for the default rule set to be run
            // https://docs.fluentvalidation.net/en/latest/rulesets.html
            return runMe.ToArray();
        }
    }
}
