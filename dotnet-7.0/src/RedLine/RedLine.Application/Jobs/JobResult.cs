using System;

namespace RedLine.Application.Jobs
{
    /// <summary>
    /// Results of a job.
    /// </summary>
    /// <remarks>When a job performs no work, a job should return <see cref="NoWorkPerformed"/>. All other successes should return <see cref="OperationSucceeded"/>. Failures should throw an <see cref="Exception"/>.</remarks>
    public struct JobResult : IEquatable<JobResult>
    {
        /// <summary>
        /// Indicates a job performed no work. No changes were made.
        /// </summary>
        /// <remarks>A job may perform queries without side effects to determine if there is any work to perform.</remarks>
        public static readonly JobResult NoWorkPerformed = new JobResult(1);

        /// <summary>
        /// Indicates a job performed some work and succeeded.
        /// </summary>
        public static readonly JobResult OperationSucceeded = new JobResult(2);

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="value">The return code value.</param>
        private JobResult(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of the <see cref="JobResult"/>, which is similar to a return code.
        /// </summary>
        /// <remarks>The value is used for equality comparison.</remarks>
        private readonly int Value { get; }

        /// <summary>
        /// Determines equality.
        /// </summary>
        /// <param name="lhs">Left hand side instance.</param>
        /// <param name="rhs">Right hand side instance.</param>
        /// <returns><c>true</c> if the instances are equal. Otherwise <c>false</c>.</returns>
        public static bool operator ==(JobResult lhs, JobResult rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines inequality.
        /// </summary>
        /// <param name="lhs">Left hand side instance.</param>
        /// <param name="rhs">Right hand side instance.</param>
        /// <returns><c>true</c> if the instances are not equal. Otherwise <c>false</c>.</returns>
        public static bool operator !=(JobResult lhs, JobResult rhs)
        {
            return !(lhs == rhs);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is JobResult jobResult && Equals(jobResult);

        /// <inheritdoc/>
        public bool Equals(JobResult other) => Value == other.Value;
    }
}
