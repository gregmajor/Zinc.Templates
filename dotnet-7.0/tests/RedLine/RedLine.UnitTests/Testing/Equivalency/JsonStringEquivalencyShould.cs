using FluentAssertions;
using RedLine.Extensions.Testing;
using Xunit;

namespace RedLine.UnitTests.Testing.Equivalency
{
    [Collection(nameof(UnitTestCollection))]
    public class JsonStringEquivalencyShould
    {
        [Fact]
        public void MatchEquivalentJsonStrings()
        {
            // Arrange
            var foo1 = new Foo
            {
                Bar = "{ foo: 123, bar: 456 }",
            };
            var foo2 = new Foo
            {
                Bar = "{ bar: 456, foo: 123  }",
            };

            // Act/Assert
            foo1.Should().NotBeEquivalentTo(foo2);
            foo1.Should().BeEquivalentTo(foo2, options => options
                .UseJsonStringEquivalency(o => o.Bar));
        }

        [Fact]
        public void NotMatchForNonEquivalentJsonStrings()
        {
            // Arrange
            var foo1 = new Foo
            {
                Bar = "{ foo: 123, bar: 456 }",
            };
            var foo2 = new Foo
            {
                Bar = "{ foo: 123, bar: 123 }",
            };

            // Act/Assert
            foo1.Should().NotBeEquivalentTo(foo2);
            foo1.Should().NotBeEquivalentTo(foo2, options => options
                .UseJsonStringEquivalency(o => o.Bar));
        }

        private class Foo
        {
            public string Bar { get; set; }
        }
    }
}
