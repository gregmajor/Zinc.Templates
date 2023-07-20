using FluentAssertions;
using RedLine.Application.Jobs;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.JobResultTests
{
    [Collection(nameof(UnitTestCollection))]
    public class GetHashCodeShould
    {
        [Fact]
        public void ReturnTheSameHashCodeWhenTheSame()
        {
            JobResult.NoWorkPerformed.GetHashCode().Should().Be(JobResult.NoWorkPerformed.GetHashCode());
            JobResult.OperationSucceeded.GetHashCode().Should().Be(JobResult.OperationSucceeded.GetHashCode());
        }

        [Fact]
        public void ReturnDifferentHashCodesWhenDifferent()
        {
            JobResult.NoWorkPerformed.GetHashCode().Should().NotBe(JobResult.OperationSucceeded.GetHashCode());
        }
    }
}
