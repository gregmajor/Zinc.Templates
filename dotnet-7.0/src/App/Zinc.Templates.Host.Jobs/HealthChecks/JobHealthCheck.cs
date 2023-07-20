using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;

namespace Zinc.Templates.Host.Jobs.HealthChecks
{
    /// <summary>
    /// A class for running health checks against Quartz jobs.
    /// </summary>
    /// <typeparam name="TJob">The type of job to monitor.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "This is by design.")]
    public class JobHealthCheck<TJob>
        where TJob : IJob
    {
        /// <summary>
        /// The last heartbeat received from the job.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "This is by design.")]
        private static long lastHeartbeat = DateTime.UtcNow.ToBinary();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="degradedThreshold">The degraded threshold.</param>
        /// <param name="unhealthyThreshold">The unhealthy threshold.</param>
        public JobHealthCheck(TimeSpan degradedThreshold, TimeSpan unhealthyThreshold)
        {
            DegradedThreshold = degradedThreshold;
            UnhealthyThreshold = unhealthyThreshold;
        }

        /// <summary>
        /// Gets the last heartbeat received from the job.
        /// </summary>
        public DateTime LastHeartbeat => DateTime.FromBinary(lastHeartbeat);

        /// <summary>
        /// Gets the threshold at which the job is considered degraded when no activity has been detected.
        /// </summary>
        public TimeSpan DegradedThreshold { get; }

        /// <summary>
        /// Gets the threshold at which the job is considered unhealthy when no activity has been detected.
        /// </summary>
        public TimeSpan UnhealthyThreshold { get; }

        /// <summary>
        /// Performs the health check.
        /// </summary>
        /// <returns><see cref="HealthCheckResult"/>.</returns>
        public Task<HealthCheckResult> CheckAsync()
        {
            var message = $"Last heartbeat received {LastHeartbeat}.";

            if (DateTime.UtcNow.Subtract(LastHeartbeat) >= UnhealthyThreshold)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(message));
            }

            if (DateTime.UtcNow.Subtract(LastHeartbeat) >= DegradedThreshold)
            {
                return Task.FromResult(HealthCheckResult.Degraded(message));
            }

            return Task.FromResult(HealthCheckResult.Healthy(message));
        }

        /// <summary>
        /// Updates the heartbeat for an instance of <typeparamref name="TJob"/>.
        /// </summary>
        internal static void Heartbeat()
        {
            Interlocked.Exchange(ref lastHeartbeat, DateTime.UtcNow.ToBinary());
        }
    }
}
