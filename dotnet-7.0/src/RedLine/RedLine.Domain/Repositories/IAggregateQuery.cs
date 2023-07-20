using RedLine.Domain.Model;

namespace RedLine.Domain.Repositories
{
    /// <summary>
    /// Marker interface for querying aggregates.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "We can't change it now.")]
    public interface IAggregateQuery<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}
