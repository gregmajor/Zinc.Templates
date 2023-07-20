using System;

namespace Zinc.Templates.Host.Jobs.Configuration
{
    /// <summary>
    /// Provides configuration settings for a job.
    /// </summary>
    internal class JobConfig
    {
        /* READ:
         * It is assumed that all jobs can share the same config structure (i.e. this class). If this turns
         * out not to be the case, then by all means feel free to rip this pattern apart and not follow it.
         * */

        /// <summary>
        /// Gets or sets the job's cron schedule.
        /// </summary>
        public string CronSchedule { get; set; }

        /// <summary>
        /// Gets or sets the threshold at which a job is considered degraded when there is no response from it.
        /// </summary>
        public TimeSpan DegradedThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the job is disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the threshold at which a job is considered unhealthy when there is no response from it.
        /// </summary>
        public TimeSpan UnhealthyThreshold { get; set; }
    }
}
