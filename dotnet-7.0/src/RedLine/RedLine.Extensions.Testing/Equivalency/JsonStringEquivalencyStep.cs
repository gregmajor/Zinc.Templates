using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Newtonsoft.Json.Linq;

namespace RedLine.Extensions.Testing.Equivalency;

internal class JsonStringEquivalencyStep<T> : IEquivalencyStep
{
    private readonly string[] names;

    public JsonStringEquivalencyStep(params Expression<Func<T, dynamic>>[] getters)
    {
        names = new string[getters.Length];

        for (var i = 0; i < getters.Length; i++)
        {
            if (getters[i].Body is not MemberExpression memberExpression)
            {
                throw new InvalidOperationException("Only simple member expressions are supported.");
            }

            if (memberExpression.Expression is not ParameterExpression)
            {
                throw new InvalidOperationException("Nested properties are not supported");
            }

            names[i] = memberExpression.Member.Name;
        }
    }

    public EquivalencyResult Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
    {
        if (comparands.CompileTimeType != typeof(string) || !names.Contains(context.CurrentNode.Name))
        {
            return EquivalencyResult.ContinueWithNext;
        }

        if (comparands.Subject is null)
        {
            comparands.Subject.Should().BeSameAs(comparands.Expectation);
        }
        else
        {
            var settings = new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore,
            };
            var subject = JObject.Parse((string)comparands.Subject, settings);
            var expectation = JObject.Parse((string)comparands.Expectation, settings);

            JToken.DeepEquals(subject, expectation).Should().BeTrue();
        }

        return EquivalencyResult.AssertionCompleted;
    }
}
