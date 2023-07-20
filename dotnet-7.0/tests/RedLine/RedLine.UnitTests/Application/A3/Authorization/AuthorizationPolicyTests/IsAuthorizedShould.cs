using System;
using FluentAssertions;
using RedLine.A3.Authorization.Domain;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authorization.AuthorizationPolicyTests
{
    public class IsAuthorizedShould : AuthorizationTestBase
    {
        private const string GenericGroup = nameof(GenericGroup);
        private const string GenericActivity = nameof(GenericActivity);
        private const string GenericResourceActivity = nameof(GenericResourceActivity);
        private const string ResourceType = nameof(ResourceType);
        private const string ResourceId = nameof(ResourceId);

        public IsAuthorizedShould(AuthorizationFixture fixture)
            : base(fixture)
        {
        }

        [Theory]
        [InlineData("ExampleTenant:Activity:GenericActivity")] // explicit activity grant
        [InlineData("*:Activity:GenericActivity")] // explicit activity grant, wildcard tenant
        [InlineData("ExampleTenant:*:GenericActivity")] // bizzare, but valid wildcard grant type
        [InlineData("ExampleTenant:Activity:*")] // Activity admin (no resource access)
        [InlineData("ExampleTenant:*:*")] // Tenant admin
        [InlineData("*:*:*")] // System admin
        public void ReturnAuthorizedForActivity(string grant)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericActivity)
                .WithGrant(grant, null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericActivity);

            // Assert
            isAuthorized.Should().BeTrue();
        }

        [Theory]
        [InlineData("")] // No activity grant
        [InlineData("*:*:OtherActivity")] // Some other activity grant
        [InlineData("Naisox:*:*")] // Tenant administrator for different tenant
        public void ReturnNotAuthorizedForActivity(string grant)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericActivity)
                .WithGrant(grant, null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericActivity);

            // Assert
            isAuthorized.Should().BeFalse();
        }

        [Theory]
        [InlineData("ExampleTenant:ResourceType:ResourceId")] // explicit resource grant
        [InlineData("*:ResourceType:ResourceId")] // resources shouldn't cross tenants.. so this probably a useless grant, but it's valid
        [InlineData("ExampleTenant:*:ResourceId")] // I sincerely hope you're not sharing ResourceIds across different aggregate resource types... but it's valid
        [InlineData("ExampleTenant:ResourceType:*")] // Resource admin for a single aggregate resource type
        [InlineData("ExampleTenant:*:*")] // Tenant admin
        [InlineData("*:*:*")] // System admin
        public void ReturnAuthorizedForResource(string resourceGrant)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericResourceActivity, ResourceType)
                .WithGrant($"{TenantId.Value}:{GrantType.Activity}:{GenericResourceActivity}", null)
                .WithGrant(resourceGrant, null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericResourceActivity, ResourceId);

            // Assert
            isAuthorized.Should().BeTrue();
        }

        [Theory]
        [InlineData("")] // no resource grant at all
        [InlineData("ExampleTenant:OtherResourceType:ResourceId")] // grant for a completely different kind of resource
        [InlineData("ExampleTenant:ResourceType:OtherResourceId")] // grant for a different resourceId
        [InlineData("Naisox:*:*")] // grant for wrong tenant
        public void ReturnNotAuthorizedForResource(string resourceGrant)
        {
            // An activity can be performed on specific resources, (see RedLine.Application.ResourceCommandBase)
            // A user is *only* authorized to perform such an activity if they are also authorized for a resource Id matching the activity's assigned ResourceType
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericResourceActivity, ResourceType)
                .WithGrant($"{TenantId.Value}:{GrantType.Activity}:{GenericResourceActivity}", null)
                .WithGrant(resourceGrant, null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericResourceActivity, ResourceId);

            // Assert
            isAuthorized.Should().BeFalse();
        }

        [Fact]
        public void ReturnNotAuthorizedWithResourceGrantButWithoutActivityGrant()
        {
            // You're authorized to act on a resource, but not authorized to do the activity.
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericResourceActivity, ResourceType)
                .WithGrant($"{TenantId.Value}:{ResourceType}:{ResourceId}", null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericResourceActivity, ResourceId);

            // Assert
            isAuthorized.Should().BeFalse();
        }

        [Theory]
        [InlineData("ExampleTenant:ActivityGroup:GenericGroup")] // explicit activity group grant
        [InlineData("*:ActivityGroup:GenericGroup")] // wildcard tenant
        [InlineData("ExampleTenant:*:GenericGroup")] // again with the strange but valid grant by type
        [InlineData("ExampleTenant:ActivityGroup:*")] // Any activity group checks out
        [InlineData("ExampleTenant:*:*")] // Tenant Admin
        [InlineData("*:*:*")] // System admin
        public void ReturnAuthorizedForActivityInGroup(string activityGroupGrant)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericActivity)
                .WithActivityGroup(GenericGroup, (TenantId.Value, GenericActivity))
                .WithGrant(activityGroupGrant, null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericActivity);

            // Assert
            isAuthorized.Should().BeTrue();
        }

        [Theory]
        [InlineData("")] // no grant at all
        [InlineData("ExampleTenant:ActivityGroup:OtherGroup")] // grant for a different group
        [InlineData("Naisox:*:*")] // grant for wrong tenant
        public void ReturnNotAuthorizedForActivityInGroup(string activityGroupGrant)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericActivity)
                .WithActivityGroup(GenericGroup, (TenantId.Value, GenericActivity))
                .WithGrant(activityGroupGrant, null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericActivity);

            // Assert
            isAuthorized.Should().BeFalse();
        }

        [Fact]
        public void ReturnAuthorizedForResourceByActivityGroup()
        {
            // This user has an activity group grant, but not the direct activity grant.
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericResourceActivity, ResourceType)
                .WithActivityGroup(GenericGroup, (TenantId.Value, GenericResourceActivity))
                .WithGrant($"{TenantId.Value}:{GrantType.ActivityGroup}:{GenericGroup}", null)
                .WithGrant($"{TenantId.Value}:{ResourceType}:{ResourceId}", null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericResourceActivity, ResourceId);

            // Assert
            isAuthorized.Should().BeTrue();
        }

        [Theory]
        [InlineData("ResourceId/path/to/sub-value")] // grant for specific sub-value
        [InlineData("ResourceId/path/to/*")] // hierarchical grant for sub-values
        [InlineData("ResourceId/path/*")] // hierarchical grant for sub-values
        [InlineData("ResourceId/*")] // all sub-value grants for ResourceId
        [InlineData("*")] // admin grant for all resources of ResourceType
        public void ReturnAuthorizedForSubResource(string subResourcePath)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericResourceActivity, ResourceType)
                .WithGrant($"{TenantId.Value}:{GrantType.Activity}:{GenericResourceActivity}", null)
                .WithGrant($"{TenantId.Value}:{ResourceType}:{subResourcePath}", null));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericResourceActivity, $"{ResourceId}/path/to/sub-value");

            // Assert
            isAuthorized.Should().BeTrue();
        }

        [Fact]
        public void ReturnAuthorizedWithExpirations()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericActivity)
                .WithGrant("*:*:*", DateTime.UtcNow.AddMinutes(1)));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericActivity);

            // Assert
            isAuthorized.Should().BeTrue();
        }

        [Fact]
        public void ReturnNotAuthorizedWithExpirations()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivity(GenericActivity)
                .WithGrant("*:*:*", DateTime.UtcNow.AddMinutes(-1)));

            // Act
            var isAuthorized = policy.IsAuthorized(GenericActivity);

            // Assert
            isAuthorized.Should().BeFalse();
        }
    }
}
