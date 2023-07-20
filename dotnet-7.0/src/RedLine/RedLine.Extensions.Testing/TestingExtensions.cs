using System;
using System.Linq.Expressions;
using Alba;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Http;
using RedLine.Extensions.Testing.Alba;
using RedLine.Extensions.Testing.Equivalency;

namespace RedLine.Extensions.Testing;

/// <summary>
/// Extension methods for tests.
/// </summary>
public static class TestingExtensions
{
    /// <summary>
    /// Use JSON Equivalency for JObject, JToken, and JArray properties.
    /// </summary>
    /// <typeparam name="T">The type being checked.</typeparam>
    /// <param name="options"><see cref="EquivalencyAssertionOptions"/>.</param>
    /// <returns>The <see cref="EquivalencyAssertionOptions"/>.</returns>
    public static EquivalencyAssertionOptions<T> UseJsonEquivalency<T>(this EquivalencyAssertionOptions<T> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.Using(new JsonEquivalencyStep());
    }

    /// <summary>
    /// Use JSON Equivalency for one or more string properties.
    /// </summary>
    /// <typeparam name="T">The type being checked.</typeparam>
    /// <param name="options"><see cref="EquivalencyAssertionOptions"/>.</param>
    /// <param name="getters">Simple expression used to define property names.</param>
    /// <returns>The <see cref="EquivalencyAssertionOptions"/>.</returns>
    public static EquivalencyAssertionOptions<T> UseJsonStringEquivalency<T>(this EquivalencyAssertionOptions<T> options, params Expression<Func<T, dynamic>>[] getters)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.Using(new JsonStringEquivalencyStep<T>(getters));
    }

    /// <summary>
    /// Use Money Equivalency for Money properties.
    /// </summary>
    /// <typeparam name="T">The type being checked.</typeparam>
    /// <param name="options"><see cref="EquivalencyAssertionOptions"/>.</param>
    /// <returns>The <see cref="EquivalencyAssertionOptions"/>.</returns>
    public static EquivalencyAssertionOptions<T> UseMoneyEquivalency<T>(this EquivalencyAssertionOptions<T> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.Using(new MoneyEquivalencyStep(4));
    }

    /// <summary>
    /// Use Money Equivalency for Money properties.
    /// </summary>
    /// <typeparam name="T">The type being checked.</typeparam>
    /// <param name="options"><see cref="EquivalencyAssertionOptions"/>.</param>
    /// <param name="precision">The precision to use during comparison.</param>
    /// <returns>The <see cref="EquivalencyAssertionOptions"/>.</returns>
    public static EquivalencyAssertionOptions<T> UseMoneyEquivalency<T>(this EquivalencyAssertionOptions<T> options, int precision)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.Using(new MoneyEquivalencyStep(precision));
    }

    /// <summary>
    /// Builds a multipart/form-data request for testing with Alba.
    /// </summary>
    /// <param name="expression">The Alba IUrlExpression.</param>
    /// <param name="payload">A dynamic object containing the form payload.
    /// Properties of type <see cref="FormFile"/> are handled as an upload. Other types are sent as a string.</param>
    /// <returns>The IUrlExpression for chaining.</returns>
    /// <example>
    /// var payload = new
    /// {
    ///     File = new FormFile(stream, 0, stream.Length, null, "blank.pdf")
    ///     {
    ///         Headers = new HeaderDictionary(),
    ///         ContentType = "application/pdf",
    ///     },
    ///     Name = "My Name"
    /// };
    ///
    /// await AuthorizedScenario(_ =>
    /// {
    ///     _.WithRequestHeader(HeaderNames.IfMatch, form.ETag);
    ///     _.Post
    ///         .MultipartFormData(payload)
    ///         .Url($"{Endpoint}/{form.FormId}");
    ///
    ///     _.StatusCodeShouldBe(204);
    ///     _.Header(HeaderNames.ETag).ShouldHaveOneNonNullValue();
    /// });
    /// </example>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "It's code.")]
    public static IUrlExpression MultipartFormData(this IUrlExpression expression, dynamic payload)
    {
        MultiPartFormDataRequestBuilder.Build(expression, payload);
        return expression;
    }
}
