using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RedLine.Domain.Model;
using RedLine.Domain.Repositories;
using Zinc.Templates.Data.DataQueries;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Queries.FindGreetings
{
    internal class FindGreetingsQueryHandler : IRequestHandler<FindGreetingsQuery, PageableResult<FindGreetingsResult>>
    {
        private readonly IRepository<Greeting> repository;
        private readonly IMapper mapper;

        public FindGreetingsQueryHandler(IRepository<Greeting> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<PageableResult<FindGreetingsResult>> Handle(FindGreetingsQuery request, CancellationToken cancellationToken)
        {
            var result = await repository.Query(new FindGreetingsDataQuery(request.Pattern)).ConfigureAwait(false);
            return new(mapper.Map<IList<FindGreetingsResult>>(result.Items));
        }
    }
}
