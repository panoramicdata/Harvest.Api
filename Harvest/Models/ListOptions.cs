namespace Harvest.Models;

/// <summary>
/// Filtering and paging for a Harvest list endpoint. Leave a property <see langword="null"/> for Harvest's default (page
/// 1, 100 per page, no date filter); pass <see langword="null"/> for the whole object to send no parameters.
/// </summary>
public class ListOptions
{
	/// <summary>Only return records updated since this date and time (<c>updated_since</c>).</summary>
	[AliasAs("updated_since")]
	public DateTime? UpdatedSince { get; init; }

	/// <summary>The page to return (<c>page</c>); Harvest's default is 1.</summary>
	[AliasAs("page")]
	public int? Page { get; init; }

	/// <summary>The number of records per page, from 1 to 2000 (<c>per_page</c>); Harvest's default is 100.</summary>
	[AliasAs("per_page")]
	public int? PerPage { get; init; }
}