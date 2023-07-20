using System;
using FluentAssertions;
using FluentAssertions.Equivalency;
using RedLine.Domain;

namespace RedLine.Extensions.Testing.Equivalency;

internal class MoneyEquivalencyStep : IEquivalencyStep
{
    private readonly int precision;

    public MoneyEquivalencyStep(int precision)
    {
        this.precision = precision;
    }

    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
    {
        if (comparands.CompileTimeType != typeof(Money))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        var subject = (Money)comparands.Subject;
        var expectation = (Money)comparands.Expectation;

        var subjectAmount = decimal.Round(subject.Amount, precision, MidpointRounding.AwayFromZero);
        var expectationAmount = decimal.Round(expectation.Amount, precision, MidpointRounding.AwayFromZero);

        subjectAmount.Should().Be(expectationAmount);
        if (subject.Amount != 0)
        {
            subject.CurrencySymbol.Should().Be(expectation.CurrencySymbol);
        }

        return EquivalencyResult.AssertionCompleted;
    }
}
