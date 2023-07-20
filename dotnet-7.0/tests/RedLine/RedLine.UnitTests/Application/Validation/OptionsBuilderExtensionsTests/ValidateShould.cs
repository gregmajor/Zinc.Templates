using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedLine.Application;
using Xunit;
using Xunit.Abstractions;

namespace RedLine.UnitTests.Application.Validation.OptionsBuilderExtensionsTests
{
    public class ValidateShould
    {
        private readonly ITestOutputHelper output;

        public ValidateShould(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static IEnumerable<object[]> OptionsThatValidate =>
            new List<object[]>
            {
                new object[] { new MyOptionsWithRuleSets() },
                new object[] { new MyOptionsWithRuleSets { RunAllRuleSets = true } },
                new object[] { new MyOptionsWithRuleSets { FailTheTestRuleSet1 = true, FailTheTestRuleSet2 = true } },
                new object[] { new MyOptionsWithRuleSets { RunRuleSet1 = true, FailTheTestRuleSet1 = false, FailTheTestRuleSet2 = true } },
            };

        public static IEnumerable<object[]> OptionsThatFail =>
            new List<object[]>
            {
                new object[] { new MyOptionsWithRuleSets { FailTheTestDefault = true }, 1, "Fail The Test Default" },
                new object[] { new MyOptionsWithRuleSets { RunAllRuleSets = true, FailTheTestDefault = true, FailTheTestRuleSet1 = true, FailTheTestRuleSet2 = true }, 3, "Fail The Test Default", "Fail The Test Rule Set1", "Fail The Test Rule Set2" },
                new object[] { new MyOptionsWithRuleSets { FailTheTestDefault = true, FailTheTestRuleSet1 = true, FailTheTestRuleSet2 = true }, 1, "Fail The Test Default" },
                new object[] { new MyOptionsWithRuleSets { RunRuleSet1 = true, FailTheTestDefault = true, FailTheTestRuleSet1 = true, FailTheTestRuleSet2 = true }, 1, "Fail The Test Rule Set1" },
                new object[] { new MyOptionsWithRuleSets { RunDefaultRuleSet = true, RunRuleSet1 = true, FailTheTestDefault = true, FailTheTestRuleSet1 = true, FailTheTestRuleSet2 = true }, 2, "Fail The Test Default", "Fail The Test Rule Set1" },
            };

        [Fact]
        public void ReturnOptionsWhenValidates()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddOptions<MyOptions>()
                .Configure(x => x.FailTheTest = false)
                .Validate<MyOptions, MyOptionsValidator>();
            var provider = serviceCollection.BuildServiceProvider();

            // act
            var options = provider.GetService<IOptions<MyOptions>>();

            // assert
            options.Value.FailTheTest.Should().BeFalse();
        }

        [Fact]
        public void ThrowValidationExceptionWhenFails()
        {
            // arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddOptions<MyOptions>()
                .Configure(x => x.FailTheTest = true)
                .Validate<MyOptions, MyOptionsValidator>();
            var provider = serviceCollection.BuildServiceProvider();

            // act & assert
            var thrownException = Assert.Throws<ValidationException>(() => _ = provider.GetService<IOptions<MyOptions>>().Value);

            thrownException.Errors.Should().HaveCount(1);
            thrownException.Errors.Single().PropertyName.Should().Be(nameof(MyOptions.FailTheTest));

            string errorMessage = thrownException.ToString();
            errorMessage.Should().Contain(typeof(MyOptions).FullName);
            errorMessage.Should().Contain(nameof(MyOptions.FailTheTest));
            this.output.WriteLine("**** exception under test thrown ****");
            this.output.WriteLine(errorMessage);
            this.output.WriteLine("**** end exception thrown ****");
        }

        [Theory]
        [MemberData(nameof(OptionsThatValidate))]
        public void ReturnOptionsWhenValidatesRuleSets(MyOptionsWithRuleSets expectedOptions)
        {
            // arrange
            var mapperConfig = new AutoMapper.MapperConfiguration(config => config.CreateMap<MyOptionsWithRuleSets, MyOptionsWithRuleSets>());
            var mapper = new AutoMapper.Mapper(mapperConfig);

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddOptions<MyOptionsWithRuleSets>()
                .Configure(x => mapper.Map(expectedOptions, x))
                .Validate<MyOptionsWithRuleSets, MyOptionsWithRuleSetsValidator>();
            var provider = serviceCollection.BuildServiceProvider();

            // act
            var options = provider.GetService<IOptions<MyOptionsWithRuleSets>>();

            // assert
            options.Value.Should().BeEquivalentTo(expectedOptions);
        }

        [Theory]
        [MemberData(nameof(OptionsThatFail))]
        public void ThrowValidationExceptionWhenFailsRuleSets(MyOptionsWithRuleSets expectedOptions, int expectedErrorCount, params string[] expectedInException)
        {
            // arrange
            var mapperConfig = new AutoMapper.MapperConfiguration(config => config.CreateMap<MyOptionsWithRuleSets, MyOptionsWithRuleSets>());
            var mapper = new AutoMapper.Mapper(mapperConfig);

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddOptions<MyOptionsWithRuleSets>()
                .Configure(x => mapper.Map(expectedOptions, x))
                .Validate<MyOptionsWithRuleSets, MyOptionsWithRuleSetsValidator>();
            var provider = serviceCollection.BuildServiceProvider();

            // act & assert
            var thrownException = Assert.Throws<ValidationException>(() => _ = provider.GetService<IOptions<MyOptionsWithRuleSets>>().Value);

            thrownException.Errors.Should().HaveCount(expectedErrorCount);

            string errorMessage = thrownException.ToString();
            errorMessage.Should().Contain(typeof(MyOptionsWithRuleSets).FullName);
            foreach (var expectedString in expectedInException)
            {
                errorMessage.Should().Contain(expectedString);
            }

            this.output.WriteLine("**** exception under test thrown ****");
            this.output.WriteLine(errorMessage);
            this.output.WriteLine("**** end exception thrown ****");
        }
    }
}
