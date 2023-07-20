using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Krypton.ActivityGroups.Domain.Events;
using NServiceBus;
using RedLine.A3.Authorization;
using RedLine.Domain;

namespace Zinc.Templates.Host.Messaging.Events.ActivityGroups
{
    /// <summary>
    /// Handles syncing changes to activity groups.
    /// </summary>
    public class ActivityGroupSyncHandler :
        IHandleMessages<ActivityAddedToGroup>,
        IHandleMessages<ActivityRemovedFromGroup>,
        IHandleMessages<ActivityGroupDeleted>
    {
        private readonly IActivityGroupService activityGroupService;
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="activityGroupService">The service for syncing activity groups.</param>
        /// <param name="connection">A connection to the database.</param>
        public ActivityGroupSyncHandler(IActivityGroupService activityGroupService, IDbConnection connection)
        {
            this.activityGroupService = activityGroupService;
            this.connection = connection;
        }

        /// <inheritdoc/>
        public async Task Handle(ActivityAddedToGroup message, IMessageHandlerContext context)
        {
            if (!message.ApplicationName.Equals(ApplicationContext.ApplicationName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using var transaction = GetTransactionScope();
            connection.Open();

            await activityGroupService.SyncActivityGroups().ConfigureAwait(false);

            transaction.Complete();
        }

        /// <inheritdoc/>
        public async Task Handle(ActivityRemovedFromGroup message, IMessageHandlerContext context)
        {
            if (!message.ApplicationName.Equals(ApplicationContext.ApplicationName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using var transaction = GetTransactionScope();
            connection.Open();

            await activityGroupService.SyncActivityGroups().ConfigureAwait(false);

            transaction.Complete();
        }

        /// <inheritdoc/>
        public async Task Handle(ActivityGroupDeleted message, IMessageHandlerContext context)
        {
            using var transaction = GetTransactionScope();
            connection.Open();

            if (!await activityGroupService.Exists(message.ActivityGroupName).ConfigureAwait(false))
            {
                return;
            }

            await activityGroupService.SyncActivityGroups().ConfigureAwait(false);

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
