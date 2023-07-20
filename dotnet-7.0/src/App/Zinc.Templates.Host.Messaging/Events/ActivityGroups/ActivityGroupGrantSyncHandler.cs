using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Krypton.ActivityGroups.Domain.Events;
using NServiceBus;
using RedLine.A3.Authorization;

namespace Zinc.Templates.Host.Messaging.Events.ActivityGroups
{
    /// <summary>
    /// Handles syncing changes to activity group grants.
    /// </summary>
    public class ActivityGroupGrantSyncHandler :
        IHandleMessages<ActivityGroupGranted>,
        IHandleMessages<ActivityGroupRevoked>
    {
        private readonly IActivityGroupService activityGroupService;
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="activityGroupService">The service for syncing activity groups.</param>
        /// <param name="connection">A connection to the database.</param>
        public ActivityGroupGrantSyncHandler(IActivityGroupService activityGroupService, IDbConnection connection)
        {
            this.activityGroupService = activityGroupService;
            this.connection = connection;
        }

        /// <inheritdoc/>
        public Task Handle(ActivityGroupGranted message, IMessageHandlerContext context) =>
            DoSync(message.UserId);

        /// <inheritdoc/>
        public Task Handle(ActivityGroupRevoked message, IMessageHandlerContext context) =>
            DoSync(message.UserId);

        private async Task DoSync(string userId)
        {
            using var transaction = GetTransactionScope();
            connection.Open();

            await activityGroupService.SyncActivityGroupGrants(userId).ConfigureAwait(false);

            transaction.Complete();
        }

        private TransactionScope GetTransactionScope() => new(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                Timeout = TimeSpan.FromMinutes(1),
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}
