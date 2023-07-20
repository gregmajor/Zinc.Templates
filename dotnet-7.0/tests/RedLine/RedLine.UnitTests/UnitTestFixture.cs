using System.Collections.Generic;
using RedLine.Domain;

namespace RedLine.UnitTests
{
    /// <summary>
    /// ApplicationContext is a static class that is used in various points in the code base.
    /// In order to make sure that it is initialized properly, we use this fixture in conjunction with <see cref="UnitTestCollection" />.
    /// xUnit ensures it creates an instance of the fixture, for every test in collection. That ensures that the static initializer
    /// on this fixture runs before the test.
    /// </summary>
    public class UnitTestFixture
    {
        public readonly Dictionary<string, string> ContextValues = StaticContextValues;

        /// <summary>
        /// The context that the ApplicationContext is initialized with.
        /// Add any other variables that should be in ApplicationContext here.
        /// Remember: ApplicationContext.Init() should be called once.
        /// </summary>
        private static readonly Dictionary<string, string> StaticContextValues = new Dictionary<string, string>
            {
                { nameof(ApplicationContext.ApplicationName), "Test" },
                { nameof(ApplicationContext.ApplicationSystemName), "Test.SystemName" },
                { "key1", "value1" },
                { "key2", "value2: '%key1%'" },
                { "key3", "value3: '%key2%'" },
                { "key4", "value4: '%key5%'" },
                { "key5", "value5: '%key4%'" },
                { nameof(ApplicationContext.AllowedClockSkew), "00:00:10" },
            };

        static UnitTestFixture()
        {
            ApplicationContext.Init(StaticContextValues);
        }
    }
}
