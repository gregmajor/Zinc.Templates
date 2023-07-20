using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Repositories;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Commands.PutGreeting
{
    internal class PutGreetingCommandHandler : IRequestHandler<PutGreetingCommand, Guid>
    {
        private readonly IRepository<Greeting> repository;

        public PutGreetingCommandHandler(IRepository<Greeting> repository)
        {
            this.repository = repository;
        }

        public async Task<Guid> Handle(PutGreetingCommand request, CancellationToken cancellationToken)
        {
            var greetingId = request.GreetingId.Equals(Guid.Empty)
                ? Guid.NewGuid()
                : request.GreetingId;

            var aggregate = await repository.Read(greetingId.ToString()).ConfigureAwait(false);

            if (aggregate == null)
            {
                aggregate = new(greetingId, request.Message, DateTimeOffset.UtcNow);
            }
            else
            {
                aggregate.UpdateMessage(request.Message);
            }

            await repository.Save(aggregate).ConfigureAwait(false);

            return aggregate.GreetingId;
        }
    }
}
