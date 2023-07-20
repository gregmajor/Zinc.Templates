using FluentAssertions;
using Newtonsoft.Json.Linq;
using RedLine.Extensions.Testing;
using Xunit;

namespace RedLine.UnitTests.Testing.Equivalency
{
    [Collection(nameof(UnitTestCollection))]
    public class JsonEquivalencyShould
    {
        [Fact]
        public void MatchEquivalentJsonStrings()
        {
            // Arrange
            var foo1 = new Foo
            {
                Bar = JObject.Parse("{ foo: 123, bar: 456 }"),
            };
            var foo2 = new Foo
            {
                Bar = JObject.Parse("{ bar: 456, foo: 123 }"),
            };

            // Act/Assert
            foo1.Should().BeEquivalentTo(foo2, options => options
                .UseJsonEquivalency());
        }

        [Fact]
        public void NotMatchForNonEquivalentJsonStrings()
        {
            // Arrange
            var foo1 = new Foo
            {
                Bar = JObject.Parse("{ foo: 123, bar: 456 }"),
            };
            var foo2 = new Foo
            {
                Bar = JObject.Parse("{ foo: 123, bar: 123 }"),
            };

            // Act/Assert
            foo1.Should().NotBeEquivalentTo(foo2, options => options
                .UseJsonEquivalency());
        }

        private class Foo
        {
            public JObject Bar { get; set; }
        }
    }
}
