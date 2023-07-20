using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using RedLine.Data.Outbox;
using RedLine.Data.Repositories;
using RedLine.Domain;
using RedLine.Domain.Model;
using RedLine.Domain.Repositories;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Data.Repositories
{
    /// <summary>
    /// Sample Repository.
    /// </summary>
    public class GreetingRepository : DbRepositoryBase<Greeting>
    {
        private readonly IDbConnection connection;
        private readonly string tenantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingRepository"/> class.
        /// </summary>
        /// <param name="context">The activity context.</param>
        /// <param name="outbox">The outbox.</param>
        public GreetingRepository(IActivityContext context, IOutbox outbox)
            : base(context, outbox)
        {
            connection = context.Connection();
            tenantId = context.TenantId();
        }

        /// <inheritdoc />
        protected override Task<int> DeleteInternal(Greeting aggregate, string etag)
        {
            // The Greeting aggregate requires ETags in our implementation! never optional!
            if (aggregate.ETag != etag)
            {
                return Task.FromResult(0);
            }

            return connection.ExecuteAsync(
                "DELETE FROM app.greetings WHERE greeting_id=@Id and etag=@ETag and tenantid=@TenantId",
                new { Id = aggregate.GreetingId, ETag = etag, TenantId = tenantId });
        }

        /// <inheritdoc />
        protected override Task<bool> ExistsInternal(string key)
        {
            return connection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS (SELECT 1 FROM app.greetings WHERE greeting_id=@Id and tenantid=@TenantId)",
                new { Id = new Guid(key), TenantId = tenantId });
        }

        /// <inheritdoc />
        protected override async Task<PageableResult<Greeting>> QueryInternal(IDbAggregateQuery<Greeting> qry)
        {
            // the ad-hoc query wants to run some sql with some Params.
            // You get to decide if that query has to Query<> or QueryMultiple and go through some child functions, etc.
            var greetings = await connection.QueryAsync<Greeting>(qry.Command, qry.Params).ConfigureAwait(false);
            return new(greetings);
        }

        /// <inheritdoc />
        protected override Task<Greeting> ReadInternal(string key)
        {
            return connection.QuerySingleOrDefaultAsync<Greeting>(
                "SELECT * FROM app.greetings WHERE greeting_id = @Id and tenantid=@TenantId",
                new { Id = new Guid(key), TenantId = tenantId });
        }

        /// <inheritdoc />
        protected override Task<Greeting> ReadInternal(IDbAggregateQuery<Greeting> qry)
        {
            return connection.QuerySingleOrDefaultAsync<Greeting>(qry.Command, qry.Params);
        }

        /// <inheritdoc />
        protected override Task<int> SaveInternal(Greeting aggregate, string etag, string newETag)
        {
            var sql = @"
                INSERT INTO app.greetings (greeting_id,message,etag,tenantid,timestamp)
                VALUES (@GreetingId,@Message,@NewETag,@TenantId,@Timestamp)
                ON CONFLICT ON CONSTRAINT greetings_pkey DO
                UPDATE SET
                    message = @Message,
                    etag = @NewETag,
                    timestamp = @Timestamp
                WHERE app.greetings.etag = @ETag";
            return connection.ExecuteAsync(
                sql,
                new
                {
                    GreetingId = aggregate.GreetingId,
                    Message = aggregate.Message,
                    TenantId = tenantId,
                    Timestamp = aggregate.Timestamp,
                    ETag = etag,
                    NewETag = newETag,
                });
        }
    }
}
