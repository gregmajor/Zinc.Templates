using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Repositories;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Commands.DeleteGreeting
{
    internal class DeleteGreetingCommandHandler : IRequestHandler<DeleteGreetingCommand>
    {
        private readonly IRepository<Greeting> repository;

        public DeleteGreetingCommandHandler(IRepository<Greeting> repository)
        {
            this.repository = repository;
        }

        public async Task<Unit> Handle(DeleteGreetingCommand request, CancellationToken cancellationToken)
        {
            var greeting = await repository.Read(request.GreetingId.ToString()).ConfigureAwait(false);
            await repository.Delete(greeting).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
