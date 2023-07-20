namespace RedLine.Domain
{
    /// <summary>
    /// The interface that defines a contract for holding state about the current activity.
    /// </summary>
    public interface IActivityContext
    {
        /// <summary>
        /// Gets a typed value from the context.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the item is not found.</param>
        /// <typeparam name="T">The type of item stored in the <see cref="IActivityContext"/>.</typeparam>
        /// <returns>The value as <typeparamref name="T"/> if found; otherwise, <paramref name="defaultValue"/>.</returns>
        T Get<T>(string key, T defaultValue);

        /// <summary>
        /// Set a typed value in the context.
        /// </summary>
        /// <param name="key">The key of the item to set.</param>
        /// <param name="value">The value as <typeparamref name="T"/>.</param>
        /// <typeparam name="T">The type of item stored in the <see cref="IActivityContext"/>.</typeparam>
        void Set<T>(string key, T value);
    }
}
