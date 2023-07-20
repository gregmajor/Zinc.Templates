using System;
using FluentAssertions;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authorization.AuthorizationPolicyTests
{
    public class HasGrantShould : AuthorizationTestBase
    {
        public HasGrantShould(AuthorizationFixture fixture)
            : base(fixture)
        {
        }

        [Theory]
        [InlineData("ExampleTenant:Activity:GenericActivity", true)] // explicit grant
        [InlineData("ExampleTenant:Activity:*", true)] // any activity will do
        [InlineData("ExampleTenant:*:*", true)] // tenant admin
        [InlineData("*:*:*", true)] // system admin
        [InlineData("ExampleTenant:ActivityGroup:GroupName", true)] // implicit grant
        [InlineData("ExampleTenant:Activity:OtherActivity", false)] // wrong explicit grant
        [InlineData("Naisox:Activity:*", false)] // wrong tenant
        [InlineData("Naisox:*:*", false)] // wrong tenant
        [InlineData("Naisox:ActivityGroup:GroupName", false)] // wrong tenant
        public void ReturnForActivity(string grant, bool shouldHaveGrant)
        {
            // Arrange
            var activity = "GenericActivity";
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithActivityGroup("GroupName", (TenantId.Value, activity))
                .WithGrant(grant, null));

            // Act
            var hasGrant = policy.HasGrant(activity);

            // Assert
            hasGrant.Should().Be(shouldHaveGrant);
        }

        [Fact]
        public void ReturnForActivityWithExpiredGrant()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant("ExampleTenant:Activity:GenericActivity", DateTime.UtcNow.AddMinutes(-1)));

            // Act
            var hasGrant = policy.HasGrant("GenericActivity");

            // Assert
            hasGrant.Should().BeFalse();
        }

        [Fact]
        public void ReturnForActivityWithFutureExpirationGrant()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant("ExampleTenant:Activity:GenericActivity", DateTime.UtcNow.AddMinutes(1)));

            // Act
            var hasGrant = policy.HasGrant("GenericActivity");

            // Assert
            hasGrant.Should().BeTrue();
        }

        [Fact]
        public void ReturnForActivityWithTypeParam()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant("ExampleTenant:Activity:HasGrantShould", DateTime.UtcNow.AddMinutes(1)));

            // Act
            var hasGrant = policy.HasGrant<HasGrantShould>();

            // Assert
            hasGrant.Should().BeTrue();
        }

        [Theory]
        [InlineData("ExampleTenant:ResourceType:ResourceId", true)] // explicit grant
        [InlineData("ExampleTenant:ResourceType:*", true)] // any ResourceId will do
        [InlineData("ExampleTenant:*:*", true)] // tenant admin
        [InlineData("*:*:*", true)] // system admin
        [InlineData("ExampleTenant:ResourceType:OtherResourceId", false)] // wrong explicit grant
        [InlineData("ExampleTenant:OtherResourceType:ResourceId", false)] // wrong explicit grant
        [InlineData("Naisox:ResourceType:*", false)] // wrong tenant
        [InlineData("Naisox:*:*", false)] // wrong tenant
        public void ReturnForResource(string grant, bool shouldHaveGrant)
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant(grant, null));

            // Act
            var hasGrant = policy.HasGrant("ResourceType", "ResourceId");

            // Assert
            hasGrant.Should().Be(shouldHaveGrant);
        }

        [Fact]
        public void ReturnForResourceWithExpiredGrant()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant("ExampleTenant:ResourceType:ResourceId", DateTime.UtcNow.AddMinutes(-1)));

            // Act
            var hasGrant = policy.HasGrant("ResourceType", "ResourceId");

            // Assert
            hasGrant.Should().BeFalse();
        }

        [Fact]
        public void ReturnForResourceWithFutureExpirationGrant()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant("ExampleTenant:ResourceType:ResourceId", DateTime.UtcNow.AddMinutes(1)));

            // Act
            var hasGrant = policy.HasGrant("ResourceType", "ResourceId");

            // Assert
            hasGrant.Should().BeTrue();
        }

        [Fact]
        public void ReturnForResourceWithTypeParam()
        {
            // Arrange
            var policy = CreateOpaPolicy().Policy(builder => builder
                .WithGrant("ExampleTenant:HasGrantShould:ResourceId", DateTime.UtcNow.AddMinutes(1)));

            // Act
            var hasGrant = policy.HasGrant<HasGrantShould>("ResourceId");

            // Assert
            hasGrant.Should().BeTrue();
        }
    }
}
