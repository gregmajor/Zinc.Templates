using RedLine.Application.Jobs;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.JobResultTests
{
    [Collection(nameof(UnitTestCollection))]
    public class InequalityOperatorShould
    {
        [Fact]
        public void ReturnTrueWhenDifferentInstances()
        {
            Assert.True(JobResult.NoWorkPerformed != JobResult.OperationSucceeded);
            Assert.True(JobResult.OperationSucceeded != JobResult.NoWorkPerformed);
        }

        [Fact]
        public void ReturnFalseWhenSameInstances()
        {
            var x1 = JobResult.NoWorkPerformed;
            var x2 = JobResult.NoWorkPerformed;
            var y1 = JobResult.OperationSucceeded;
            var y2 = JobResult.OperationSucceeded;
            Assert.False(x1 != x2);
            Assert.False(y1 != y2);
        }
    }
}
