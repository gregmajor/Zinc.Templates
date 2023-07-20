using System;

namespace RedLine.Domain.Model
{
    /// <summary>
    /// An abstract base class for entities.
    /// </summary>
    public abstract class EntityBase : IEntity, IEquatable<IEntity>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected EntityBase()
        { }

        /// <summary>
        /// Gets or sets the business-centric key or id of the entity, not to be confused with a database primary key.
        /// </summary>
        public abstract string Key { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as IEntity);
        }

        /// <inheritdoc/>
        public virtual bool Equals(IEntity other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.GetType().Equals(GetType()))
            {
                return other.Key.Equals(Key);
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 571 ^ GetType().GetHashCode() ^ Key.GetHashCode();
        }
    }
}
