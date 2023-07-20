using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.A3;
using RedLine.Domain.Repositories;
using Zinc.Templates.Data.DataQueries.UXGreetingsScreen;

namespace Zinc.Templates.Application.Queries.UXGreetingsScreen.UXGreeting
{
    internal class UXGreetingQueryHandler : IRequestHandler<UXGreetingQuery, UXGreetingResult>
    {
        private readonly IRepository repository;
        private readonly IAccessToken currentUser;

        public UXGreetingQueryHandler(IRepository repository, IAccessToken currentUser)
        {
            this.repository = repository;
            this.currentUser = currentUser;
        }

        public async Task<UXGreetingResult> Handle(UXGreetingQuery request, CancellationToken cancellationToken)
        {
            var (message, etag, timestamp) = await repository.Query(new UXGreetingDataQuery(request.GreetingId, request.TenantId)).ConfigureAwait(false);

            return new()
            {
                CanUpdateGreetings = currentUser.HasGrant<Commands.PutGreeting.PutGreetingCommand>(),
                Message = message,
                ETag = etag,
                Timestamp = timestamp,
            };
        }
    }
}
