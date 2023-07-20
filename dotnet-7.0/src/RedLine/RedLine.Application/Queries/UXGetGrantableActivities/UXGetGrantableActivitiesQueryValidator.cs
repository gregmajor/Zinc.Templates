namespace RedLine.Application.Queries.UXGetGrantableActivities
{
    /// <summary>
    /// Validator for <see cref="UXGetGrantableActivitiesQuery"/>.
    /// </summary>
    public class UXGetGrantableActivitiesQueryValidator : QueryValidator<UXGetGrantableActivitiesQuery>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="UXGetGrantableActivitiesQuery"/>.
        /// </summary>
        public UXGetGrantableActivitiesQueryValidator()
        {
            RuleFor(x => x.UserId).NotNullOrWhitespace();
        }
    }
}
