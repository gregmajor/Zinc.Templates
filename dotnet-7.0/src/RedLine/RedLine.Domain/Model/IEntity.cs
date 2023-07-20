namespace RedLine.Domain.Model
{
    /// <summary>
    /// The interface that defines a contract for entities.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets the business-centric key or id of the entity, not to be confused with a database primary key.
        /// </summary>
        string Key { get; }
    }
}
