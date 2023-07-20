using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RedLine.Domain
{
    /// <summary>
    /// Helper methods for <see cref="CultureInfo"/>.
    /// </summary>
    public static class Cultures
    {
        private static readonly Dictionary<string, CultureInfo> ValidCultures = CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .ToDictionary(c => c.Name, c => c, StringComparer.Ordinal);

        /// <summary>
        /// Names of valid cultures.
        /// </summary>
        public static IEnumerable<string> Names => ValidCultures.Keys;

        /// <summary>
        /// Determines if a culture name is valid or not.
        /// </summary>
        /// <param name="cultureName">The culture name to validate.</param>
        /// <returns><c>true</c> if the culture is valid. Otherwise, <c>false</c>.</returns>
        public static bool IsValidCultureName(string cultureName)
        {
            if (cultureName == null)
            {
                return false;
            }

            return ValidCultures.ContainsKey(cultureName);
        }

        /// <summary>
        /// Gets the culture by name.
        /// </summary>
        /// <param name="name">The name of the culture.</param>
        /// <returns>The <see cref="CultureInfo"/> with the corresponding name, assuming the name is valid.</returns>
        public static CultureInfo GetCulture(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !ValidCultures.TryGetValue(name, out var cultureInfo))
            {
                throw new CultureNotFoundException(
                    message: $"The requested culture {name} is not available.",
                    invalidCultureName: name,
                    innerException: null);
            }

            return cultureInfo;
        }
    }
}
