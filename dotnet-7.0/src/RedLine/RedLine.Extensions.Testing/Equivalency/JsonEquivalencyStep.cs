using FluentAssertions;
using FluentAssertions.Equivalency;
using Newtonsoft.Json.Linq;

namespace RedLine.Extensions.Testing.Equivalency;

internal class JsonEquivalencyStep : IEquivalencyStep
{
    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
    {
        if (!comparands.CompileTimeType.IsAssignableTo(typeof(JToken)))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        if (comparands.Subject is null)
        {
            comparands.Subject.Should().BeSameAs(comparands.Expectation);
        }
        else
        {
            JToken.DeepEquals((JToken)comparands.Subject, (JToken)comparands.Expectation).Should().BeTrue();
        }

        return EquivalencyResult.AssertionCompleted;
    }
}
