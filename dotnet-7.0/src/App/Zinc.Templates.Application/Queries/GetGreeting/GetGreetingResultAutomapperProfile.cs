using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Queries.GetGreeting
{
    /// <summary>
    /// An AutoMapper profile that specifies mapping rules.
    /// </summary>
    public class GetGreetingResultAutomapperProfile : AutoMapper.Profile
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GetGreetingResultAutomapperProfile()
        {
            CreateMap<Greeting, GetGreetingResult>();
        }
    }
}
