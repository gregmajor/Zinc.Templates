using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RedLine.Domain.Repositories;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Queries.GetGreeting
{
    internal class GetGreetingQueryHandler : IRequestHandler<GetGreetingQuery, GetGreetingResult>
    {
        private readonly IRepository<Greeting> repository;
        private readonly IMapper mapper;

        public GetGreetingQueryHandler(IRepository<Greeting> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<GetGreetingResult> Handle(GetGreetingQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.Read(request.GreetingId.ToString()).ConfigureAwait(false);
            return mapper.Map<GetGreetingResult>(result);
        }
    }
}
