using System;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.Domain.Model
{
    /// <summary>
    /// Represents a pageable list of items from a query. The use of paging is optional.
    /// </summary>
    /// <typeparam name="T">The type of item returned.</typeparam>
    public class PageableResult<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageableResult{T}"/> class.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <param name="page">The page of results to return, set by the client and echoed back by the server.</param>
        /// <param name="pageSize">The size of each page of results, set by the client and echoed back by the server.</param>
        /// <param name="totalItems">The total number of items in the data store, set by the server.</param>
        /// <remarks>This ctor is typically used when paging is in effect.</remarks>
        public PageableResult(IEnumerable<T> items, int? page, int? pageSize, long? totalItems)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (items is IList<T> list)
            {
                Items = list;
            }
            else
            {
                Items = items.ToList();
            }

            if (page.HasValue && pageSize.HasValue)
            {
                if (page <= 0)
                {
                    throw new ArgumentException($"The '{nameof(page)}' argument must be greater than zero, and must be used in conjunction with the '{pageSize}' argument.", nameof(page));
                }

                if (pageSize <= 0)
                {
                    throw new ArgumentException($"The '{nameof(pageSize)}' argument must be greater than zero, and must be used in conjunction with the '{page}' argument.", nameof(pageSize));
                }
            }

            if (page == null && pageSize != null)
            {
                throw new ArgumentException($"The '{nameof(page)}' argument must be greater than zero, and must be used in conjunction with the '{pageSize}' argument.", nameof(page));
            }

            if (page != null && pageSize == null)
            {
                throw new ArgumentException($"The '{nameof(pageSize)}' argument must be greater than zero, and must be used in conjunction with the '{page}' argument.", nameof(pageSize));
            }

            if (totalItems.HasValue && Items.Count > totalItems)
            {
                throw new ArgumentException($"The '{nameof(totalItems)}' argument must be greater than or equal to the count of '{items}'.", nameof(totalItems));
            }

            Page = page ?? 1;
            PageSize = pageSize ?? Items.Count;
            TotalItems = totalItems ?? Items.Count;

            TotalPages = TotalItems > 0 && PageSize > 0
                             ? (int)Math.Ceiling(decimal.Divide(new decimal(TotalItems), new decimal(PageSize)))
                             : 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageableResult{T}"/> class.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <remarks>This ctor is used when paging is NOT in effect.</remarks>
        public PageableResult(IEnumerable<T> items)
            : this(items, null, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the object.
        /// </summary>
        protected PageableResult()
        {
        }

        /// <summary>
        /// Gets or sets the page of items returned from the query.
        /// </summary>
        public IList<T> Items { get; protected set; }

        /// <summary>
        /// Gets or sets the page number of the results to return, set by the client.
        /// </summary>
        public int Page { get; protected set; }

        /// <summary>
        /// Gets or sets the size of each page to return, set by the client.
        /// </summary>
        public int PageSize { get; protected set; }

        /// <summary>
        /// Gets or sets the total number of items in the result set, set by the server.
        /// </summary>
        public long TotalItems { get; protected set; }

        /// <summary>
        /// The total page count in the result set, based on the values of TotalItems and PageSize.
        /// </summary>
        public int TotalPages { get; protected set; }

        /// <summary>
        /// Whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Whether there is next page.
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Whether this page is the first page.
        /// </summary>
        public bool IsFirstPage => Page == 1;

        /// <summary>
        /// Whether this page is the last page.
        /// </summary>
        public bool IsLastPage => Page == TotalPages;

        /// <summary>
        /// Index accessor.
        /// </summary>
        /// <param name="index">The index to access.</param>
        public T this[int index] => Items[index];

        /// <summary>
        /// Enumerate the results.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    }
}
