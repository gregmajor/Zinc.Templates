using RedLine.Application.Jobs;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.JobResultTests
{
    [Collection(nameof(UnitTestCollection))]
    public class EqualityOperatorShould
    {
        [Fact]
        public void ReturnFalseWhenDifferentInstances()
        {
            Assert.False(JobResult.NoWorkPerformed == JobResult.OperationSucceeded);
            Assert.False(JobResult.OperationSucceeded == JobResult.NoWorkPerformed);
        }

        [Fact]
        public void ReturnTrueWhenSameInstances()
        {
            var x1 = JobResult.NoWorkPerformed;
            var x2 = JobResult.NoWorkPerformed;
            var y1 = JobResult.OperationSucceeded;
            var y2 = JobResult.OperationSucceeded;
            Assert.True(x1 == x2);
            Assert.True(y1 == y2);
        }
    }
}
