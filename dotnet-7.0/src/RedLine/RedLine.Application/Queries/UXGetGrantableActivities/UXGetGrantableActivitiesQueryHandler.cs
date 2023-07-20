using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.A3.Authorization.Domain;
using RedLine.Domain.A3.Authorization.Repositories;

namespace RedLine.Application.Queries.UXGetGrantableActivities
{
    /// <summary>
    /// Handles the <see cref="UXGetGrantableActivitiesQuery" />.
    /// </summary>
    public class UXGetGrantableActivitiesQueryHandler : IRequestHandler<UXGetGrantableActivitiesQuery, IEnumerable<UXGrantableActivity>>
    {
        private readonly IGrantRepository repository;
        private readonly IEnumerable<IActivity> activities;

        /// <summary>
        /// Initializes the handler.
        /// </summary>
        /// <param name="repository">The grant repository.</param>
        /// <param name="activities">The activities in the application.</param>
        public UXGetGrantableActivitiesQueryHandler(
            IGrantRepository repository,
            IEnumerable<IActivity> activities)
        {
            this.repository = repository;
            this.activities = activities;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UXGrantableActivity>> Handle(UXGetGrantableActivitiesQuery request, CancellationToken cancellationToken)
        {
            var filter = new GrantScope(request.TenantId, GrantType.Activity, "%");

            var activityGrants = (await repository.Matching(filter, request.UserId).ConfigureAwait(false))
                .Where(g => g.IsActive())
                .ToDictionary(key => key.Scope.Qualifier);

            return activities.Select(a =>
            {
                activityGrants.TryGetValue(a.GetType().Name, out var grant);

                return new UXGrantableActivity
                {
                    Name = a.ActivityName,
                    DisplayName = a.ActivityDisplayName,
                    Description = a.ActivityDescription,
                    ExpiresOn = grant?.ExpiresOn,
                    GrantedBy = grant?.GrantedBy,
                    GrantedOn = grant?.GrantedOn,
                };
            });
        }
    }
}
