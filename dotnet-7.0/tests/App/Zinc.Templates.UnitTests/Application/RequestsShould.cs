using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation;
using RedLine.Application;
using RedLine.Domain;
using Xunit;

namespace Zinc.Templates.UnitTests.Application
{
    public abstract class RequestsShould
    {
        protected static readonly IEnumerable<Type> AllImplementations =
            typeof(Zinc.Templates.Application.AssemblyMarker).Assembly.GetTypes()
                .Concat(typeof(RedLine.Application.AssemblyMarker).Assembly.GetTypes())
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsGenericType)
                .ToArray();

        protected static readonly IEnumerable<Type> AllValidators = AllImplementations
            .Where(t => t.IsAssignableTo(typeof(IValidator)))
            .ToArray();
    }

    public abstract class RequestsShould<TInterface> : RequestsShould
    {
        private static IEnumerable<Type> RequestTypes => AllImplementations
            .Where(t => t.IsAssignableTo(typeof(TInterface)))
            .ToArray();

        public static object[][] GetRequestTypesThatShouldBeValidated()
        {
            return RequestTypes
                .Where(t => t.GetFields().Any() || t.GetProperties().Any())
                .Concat(new[] { typeof(object) })
                .Select(t => new[] { (object)t })
                .ToArray();
        }

        public static object[][] GetRequestTypesThatShouldUseStandardValidators()
        {
            return RequestTypes
                .Where(t => t.IsAssignableTo(typeof(IActivity)) ||
                            t.IsAssignableTo(typeof(IAmMultiTenant)) ||
                            t.IsAssignableTo(typeof(ICorrelationId)) ||
                            t.IsAssignableTo(typeof(IAmAuthorizableForResource)))
                .Concat(new[] { typeof(object) })
                .Select(t => new[] { (object)t })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetRequestTypesThatShouldBeValidated))]
        public void HaveAValidator(Type commandType)
        {
            if (commandType == typeof(object))
            {
                // avoids issue where theory's member data doesn't return any types.
                return;
            }

            var validator = typeof(IValidator<>).MakeGenericType(commandType);

            AllValidators
                .Where(t => t.IsAssignableTo(validator))
                .Should().NotBeEmpty($"Unable to find a validator for {commandType.FullName}");
        }
    }
}
