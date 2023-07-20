using System.Diagnostics.CodeAnalysis;
using FluentMigrator;
using RedLine.A3.Authorization;

namespace RedLine.Data.Migrations.Migrations
{
    /// <summary>
    /// Sync the activity group grants for all users.
    /// </summary>
    [Migration(2022010601)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are name according to our standards.")]
    public class Migration_2022010601_SyncActivityGrantsForUsers : ForwardOnlyMigration
    {
        private readonly IActivityGroupService activityGroupService;

        /// <summary>
        /// Initializes an instance of <see cref="Migration_2022010601_SyncActivityGrantsForUsers" /> class.
        /// </summary>
        /// <param name="activityGroupService">The service for syncing activity groups.</param>
        public Migration_2022010601_SyncActivityGrantsForUsers(IActivityGroupService activityGroupService)
        {
            this.activityGroupService = activityGroupService;
        }

        /// <inheritdoc />
        public override void Up()
        {
            activityGroupService.SyncActivityGroups().Wait();
            activityGroupService.SyncAllActivityGroupGrants().Wait();
        }
    }
}
