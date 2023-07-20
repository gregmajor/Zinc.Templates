using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Application.Queries.FindGreetings
{
    /// <summary>
    /// An AutoMapper profile that specifies mapping rules.
    /// </summary>
    public class FindGreetingResultAutomapperProfile : AutoMapper.Profile
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FindGreetingResultAutomapperProfile()
        {
            CreateMap<Greeting, FindGreetingsResult>();
        }
    }
}
