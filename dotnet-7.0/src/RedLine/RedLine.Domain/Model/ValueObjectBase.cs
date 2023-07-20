namespace RedLine.Domain.Model
{
    /// <summary>
    /// An abstract base class for value objects.
    /// </summary>
    public abstract class ValueObjectBase : IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected ValueObjectBase()
        {
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => EqualsInternal(obj as IValueObject);

        /// <inheritdoc/>
        public override int GetHashCode() => 761 ^ GetType().GetHashCode() ^ GetHashCodeInternal();

        /// <summary>
        /// An abstract method used to determine if this object is equal to another.
        /// </summary>
        /// <param name="other">The other value object to compare to this one.</param>
        /// <returns>True if the two objects are equal; otherwise, false.</returns>
        protected abstract bool EqualsInternal(IValueObject other);

        /// <summary>
        /// An abstract method used to retrieve a hash code, which is used for equality testing.
        /// </summary>
        /// <returns>The hash code as an integer.</returns>
        protected abstract int GetHashCodeInternal();
    }
}
