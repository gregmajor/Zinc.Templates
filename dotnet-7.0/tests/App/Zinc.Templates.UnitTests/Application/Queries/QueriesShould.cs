using System;
using System.Linq;
using FluentAssertions;
using RedLine.Application.Queries;
using Xunit;

namespace Zinc.Templates.UnitTests.Application.Queries
{
    public class QueriesShould : RequestsShould<IQuery>
    {
        [Theory]
        [MemberData(nameof(GetRequestTypesThatShouldUseStandardValidators))]
        public void UseStandardValidators(Type queryType)
        {
            if (queryType == typeof(object))
            {
                // avoids issue where theory's member data doesn't return any types.
                return;
            }

            var validator = typeof(QueryValidator<>).MakeGenericType(queryType);

            AllValidators
                .Where(t => t.IsAssignableTo(validator))
                .Should().NotBeEmpty($"Query must have a validator that inherits {typeof(QueryValidator<>).FullName}");
        }

        [Theory]
        [MemberData(nameof(GetRequestTypesThatShouldUseStandardValidators))]
        public void FollowNamingConvention(Type queryType)
        {
            if (queryType == typeof(object))
            {
                // avoids issue where theory's member data doesn't return any types.
                return;
            }

            queryType.Name.Should().EndWith("Query");

            if (queryType.Name.StartsWith("UX", StringComparison.InvariantCultureIgnoreCase))
            {
                queryType.Name.Should().StartWith("UX", "UX should be capitalized");
            }
        }
    }
}
