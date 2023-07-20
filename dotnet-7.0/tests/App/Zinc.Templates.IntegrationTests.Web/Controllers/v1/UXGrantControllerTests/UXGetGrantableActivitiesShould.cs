using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Application;
using RedLine.Application.Queries.UXGetGrantableActivities;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Host.Web.Models.Authorization;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.UXGrantControllerTests
{
    public class UXGetGrantableActivitiesShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/activities/grants";
        private readonly List<IActivity> activities;

        public UXGetGrantableActivitiesShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            activities = GetRequiredService<IEnumerable<IActivity>>().ToList();
        }

        [Fact]
        public async Task ReturnAllActivities()
        {
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}/some-user-id");
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            var result = response.ReadAsJson<IEnumerable<UXGrantableActivity>>();
            result.Should().NotBeNull();
            result.Should().HaveCount(activities.Count);
            activities.ForEach(a =>
                result.Should().Contain(r =>
                    r.Name == a.ActivityName &&
                    r.DisplayName == a.ActivityDisplayName &&
                    r.Description == a.ActivityDescription &&
                    r.GrantedBy == null &&
                    r.GrantedOn == null &&
                    r.ExpiresOn == null));
        }

        [Fact]
        public async Task ReturnGrantedActivities()
        {
            var model = new AddGrantModel { ExpiresOn = null, FullName = "Some User Name" };

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl($"/api/v1/{TenantId}/activities/{nameof(UXGetGrantableActivitiesQuery)}/grants/some-user-id");
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}/some-user-id");
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            var result = response.ReadAsJson<IEnumerable<UXGrantableActivity>>();
            result.Should().NotBeNull();
            result.Should().HaveCount(activities.Count);
            result.Should().Contain(a => a.GrantedOn.HasValue && a.Name == nameof(UXGetGrantableActivitiesQuery));
        }
    }
}
