using System;
using FluentAssertions;
using RedLine.Application.Jobs;
using Xunit;

namespace RedLine.UnitTests.Application.Jobs.JobResultTests
{
    [Collection(nameof(UnitTestCollection))]
    public class EqualsShould
    {
        [Fact]
        public void ReturnEqualsWhenTheSame()
        {
            JobResult.NoWorkPerformed.Equals(JobResult.NoWorkPerformed).Should().BeTrue();
            JobResult.OperationSucceeded.Equals(JobResult.OperationSucceeded).Should().BeTrue();

            JobResult.NoWorkPerformed.Equals((object)JobResult.NoWorkPerformed).Should().BeTrue();
            JobResult.OperationSucceeded.Equals((object)JobResult.OperationSucceeded).Should().BeTrue();

            JobResult.OperationSucceeded.Equals((object)JobResult.OperationSucceeded).Should().BeTrue();

            var x1 = JobResult.NoWorkPerformed;
            var x2 = JobResult.NoWorkPerformed;
            var y1 = JobResult.OperationSucceeded;
            var y2 = JobResult.OperationSucceeded;
            object.Equals(x1, x2).Should().BeTrue();
            object.Equals(y1, y2).Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseWhenDifferent()
        {
            JobResult.NoWorkPerformed.Equals(JobResult.OperationSucceeded).Should().BeFalse();
            JobResult.OperationSucceeded.Equals(JobResult.NoWorkPerformed).Should().BeFalse();

            JobResult.NoWorkPerformed.Equals((object)JobResult.OperationSucceeded).Should().BeFalse();
            JobResult.OperationSucceeded.Equals((object)JobResult.NoWorkPerformed).Should().BeFalse();

            object.Equals(JobResult.NoWorkPerformed, JobResult.OperationSucceeded).Should().BeFalse();
            object.Equals(JobResult.OperationSucceeded, JobResult.NoWorkPerformed).Should().BeFalse();
        }

        [Fact]
        public void ReturnFalseWhenNull()
        {
            JobResult.NoWorkPerformed.Equals(null).Should().BeFalse();
            JobResult.OperationSucceeded.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void ReturnFalseWhenNotAJobResult()
        {
            JobResult.NoWorkPerformed.Equals(new object()).Should().BeFalse();
            JobResult.OperationSucceeded.Equals(new object()).Should().BeFalse();
            JobResult.NoWorkPerformed.Equals(DateTime.UtcNow).Should().BeFalse();
            JobResult.OperationSucceeded.Equals(DateTime.UtcNow).Should().BeFalse();
        }

        [Fact]
        public void ReturnFalseWhenDefault()
        {
            JobResult.NoWorkPerformed.Equals(default(JobResult)).Should().BeFalse();
            JobResult.OperationSucceeded.Equals(default(JobResult)).Should().BeFalse();
        }
    }
}
