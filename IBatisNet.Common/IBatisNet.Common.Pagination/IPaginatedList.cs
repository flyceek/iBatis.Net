using System.Collections;

namespace IBatisNet.Common.Pagination
{
	/// <summary>
	/// Summary description for IPaginatedList.
	/// </summary>
	public interface IPaginatedList : IList, ICollection, IEnumerable, IEnumerator
	{
		/// <summary>
		/// The maximum number of items per page.
		/// </summary>
		int PageSize
		{
			get;
		}

		/// <summary>
		/// Is the current page the first page ?
		/// True if the current page is the first page or if only
		/// a single page exists.
		/// </summary>
		bool IsFirstPage
		{
			get;
		}

		/// <summary>
		/// Is the current page a middle page (i.e. not first or last) ?
		/// Return True if the current page is not the first or last page,
		/// and more than one page exists (always returns false if only a
		/// single page exists).
		/// </summary>
		bool IsMiddlePage
		{
			get;
		}

		/// <summary>
		/// Is the current page the last page ?
		/// Return True if the current page is the last page or if only
		/// a single page exists.
		/// </summary>
		bool IsLastPage
		{
			get;
		}

		/// <summary>
		/// Is a page available after the current page ?
		/// Return True if the next page is available
		/// </summary>
		bool IsNextPageAvailable
		{
			get;
		}

		/// <summary>
		/// Is a page available before the current page ?
		/// Return True if the previous page is available
		/// </summary>
		bool IsPreviousPageAvailable
		{
			get;
		}

		/// <summary>
		/// Returns the current page index, which is a zero based integer.
		/// All paginated list implementations should know what index they are
		/// on, even if they don't know the ultimate boundaries (min/max)
		/// </summary>
		int PageIndex
		{
			get;
		}

		/// <summary>
		/// Moves to the next page after the current page.  If the current
		/// page is the last page, wrap to the first page.
		/// </summary>
		/// <returns></returns>
		bool NextPage();

		/// <summary>
		/// Moves to the page before the current page.  If the current
		/// page is the first page, wrap to the last page.
		/// </summary>
		/// <returns></returns>
		bool PreviousPage();

		/// <summary>
		/// Moves to a specified page.  If the specified
		/// page is beyond the last page, wrap to the first page.
		/// If the specified page is before the first page, wrap
		/// to the last page.
		/// </summary>
		/// <param name="pageIndex">The index of the specified page.</param>
		void GotoPage(int pageIndex);
	}
}
