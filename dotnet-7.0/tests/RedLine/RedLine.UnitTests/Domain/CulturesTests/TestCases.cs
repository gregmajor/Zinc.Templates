using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.UnitTests.Domain.CulturesTests
{
    public abstract class TestCases<T> : IEnumerable<object[]>
    {
        public abstract IEnumerable<T> GetTestCases();

        public IEnumerator<object[]> GetEnumerator()
        {
            return GetTestCases()
                .Select(tc => new[] { (object)tc })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
