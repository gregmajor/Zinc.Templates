using System;
using System.Collections.Generic;

namespace RedLine.Domain
{
    /// <summary>
    /// Extensions for the built in IEnumerable.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Convenience to use IList's ForEach() without realizing the enumerable in advance.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to perform on every item in the enumerable.</param>
        /// <typeparam name="T">The type of items in the enumerable.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var x in enumerable)
            {
                action(x);
            }
        }
    }
}
