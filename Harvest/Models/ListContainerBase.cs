namespace Harvest.Models;

/// <summary>
/// The paging envelope of every Harvest list response. The records themselves are in the derived container's list
/// property, named after the JSON key that holds them (for example <c>clients</c>).
/// </summary>
public abstract class ListContainerBase
{
	/// <summary>The number of records per page.</summary>
	public int PerPage { get; set; }

	/// <summary>The total number of records across all pages.</summary>
	public int TotalEntries { get; set; }

	/// <summary>The total number of pages.</summary>
	public int TotalPages { get; set; }

	/// <summary>The next page number, or <see langword="null"/> on the last page (and for most cursor-paged responses).</summary>
	public int? NextPage { get; set; }

	/// <summary>The previous page number, or <see langword="null"/> on the first page (and for most cursor-paged responses).</summary>
	public int? PreviousPage { get; set; }

	/// <summary>The current page number.</summary>
	public int? Page { get; set; }

	/// <summary>
	/// The URLs of the first, next, previous and last pages. Harvest recommends following these rather than building page
	/// requests; <see cref="PaginationLinks.NextCursor"/> gives the cursor to pass as <see cref="ListOptions.Cursor"/>.
	/// </summary>
	public PaginationLinks? Links { get; set; }
}