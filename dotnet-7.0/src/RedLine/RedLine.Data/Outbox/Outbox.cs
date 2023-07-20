using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RedLine.Domain;
using RedLine.Domain.Events;
using RedLine.Domain.Model;

namespace RedLine.Data.Outbox
{
    /// <inheritdoc/>
    internal class Outbox : IOutbox
    {
        private readonly IActivityContext context;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The <see cref="IActivityContext"/> for the current request.</param>
        public Outbox(IActivityContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<int> DispatchEvents(string dispatcherId, Func<OutboxRecord, Task<int>> dispatch)
        {
            var connection = context.Connection();

            var outboxRecord = await connection.QueryFirstOrDefaultAsync<OutboxRecord>(
                Sql.ReserveOutboxRecord,
                new { dispatcherId }).ConfigureAwait(false);

            if (outboxRecord == null)
            {
                return 0;
            }

            var result = await dispatch(outboxRecord).ConfigureAwait(false);

            await connection.ExecuteAsync(
                Sql.DeleteOutboxRecord,
                new { outboxRecord.Id }).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc/>
        public async Task<int> SaveEvents(IAggregateRoot aggregate)
        {
            if (!aggregate.Events.Any())
            {
                return 0;
            }

            var messages = aggregate.Events.Select(evt => new OutboxMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                MessageBody = evt,
                MessageHeaders = GetDefaultHeaders(),
            }).ToList();

            var result = await SaveMessages(messages).ConfigureAwait(false);

            (aggregate.Events as ICollection<IDomainEvent>)?.Clear();

            return result;
        }

        /// <inheritdoc />
        public Task<int> SaveCommandToThisApplication(object command) => SaveCommand(command, ApplicationContext.ApplicationName);

        /// <inheritdoc />
        public async Task<int> SaveCommand(object command, string destination)
        {
            ValidateCommandParameters(command, destination);

            var message = new OutboxMessage
            {
                Destination = destination,
                MessageId = Guid.NewGuid().ToString(),
                MessageBody = command,
                MessageHeaders = GetDefaultHeaders(),
            };

            return await SaveMessages(new[] { message });
        }

        /// <inheritdoc/>
        public async Task<int> SaveMessages(IEnumerable<OutboxMessage> messages)
        {
            if (messages.Any())
            {
                await context
                    .Connection()
                    .ExecuteAsync(Sql.Insert, new OutboxRecord
                    {
                        Id = Guid.NewGuid(),
                        Messages = messages,
                    }).ConfigureAwait(false);

                return messages.Count();
            }

            return 0;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> GetDefaultHeaders() =>
            new Dictionary<string, string>
            {
                { RedLineHeaderNames.CorrelationId, context.CorrelationId().ToString() },
                { RedLineHeaderNames.TenantId, context.TenantId() },
            };

        private void ValidateCommandParameters(object command, string destination)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentNullException(nameof(destination));
            }
        }

        internal static class Sql
        {
            internal static readonly string TableName = "outbox.outbox";
            internal static readonly int DispatcherTimeoutMilliseconds = 5000;

            /// <summary>
            /// Deletes an <see cref="OutboxRecord"/> after it has been dispatched.
            /// </summary>
            internal static readonly string DeleteOutboxRecord = $@"
DELETE FROM {TableName} WHERE id = @Id";

            /// <summary>
            /// Inserts an <see cref="OutboxRecord"/> to be dispatched.
            /// </summary>
            internal static readonly string Insert = $@"
INSERT INTO {TableName} (id, messages) VALUES (@Id, @Messages::jsonb);";

            /// <summary>
            /// Reserves an <see cref="OutboxRecord"/> for dispatching.
            /// </summary>
            /// <remarks>
            /// This SQL includes "FOR UPDATE SKIP LOCKED" in order to prevent a race condition.
            /// See https://tnishimura.github.io/articles/queues-in-postgresql/.
            /// </remarks>
            internal static readonly string ReserveOutboxRecord = $@"
UPDATE {TableName} as earliest
SET
    dispatcher_id = @dispatcherId,
    dispatcher_timeout = now() + '{DispatcherTimeoutMilliseconds} milliseconds' 
WHERE earliest.sid =
( 
    SELECT sid FROM {TableName} as other 
    WHERE (other.dispatcher_id = @dispatcherId AND now() <= other.dispatcher_timeout)
        OR other.dispatcher_id IS NULL
        OR now() > other.dispatcher_timeout
    ORDER BY sid FOR UPDATE SKIP LOCKED LIMIT 1
);

SELECT
    id,
    dispatcher_id,
    dispatcher_timeout,
    messages::json
FROM {TableName} WHERE dispatcher_id = @dispatcherId ORDER BY sid LIMIT 1;
";
        }
    }
}
