using RedLine.Data.Repositories;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;

namespace Zinc.Templates.Data.DataQueries
{
    /// <summary>
    /// Query a greeting by its message.
    /// </summary>
    public class FindGreetingsDataQuery : DbAggregateQueryBase<Greeting>
    {
        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="pattern">The pattern to search by.</param>
        public FindGreetingsDataQuery(string pattern)
        {
            Params = new { pattern };
            Command = "SELECT * FROM app.greetings WHERE message LIKE @pattern";
        }
    }
}
