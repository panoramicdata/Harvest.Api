namespace Harvest.Models;

/// <summary>
/// Filtering and paging for a Harvest list endpoint. Leave a property <see langword="null"/> for Harvest's default (first
/// page, 2000 per page, no filter); pass <see langword="null"/> for the whole object to send no parameters. Endpoints with
/// extra filters take a derived options class (for example <see cref="TimeEntryListOptions"/>).
/// </summary>
/// <remarks>
/// Harvest prefers cursor pagination: request the first page, then, while the previous response's
/// <see cref="PaginationLinks.NextCursor"/> is set, pass it as <see cref="Cursor"/>. Where Harvest answers with
/// page-numbered links instead (<see cref="PaginationLinks.NextCursor"/> is <see langword="null"/>), pass
/// <see cref="ListContainerBase.NextPage"/> as <see cref="Page"/> until it is <see langword="null"/>. Harvest documents
/// <see cref="Page"/> as deprecated; it still works, and the two are mutually exclusive (the cursor wins if both are set).
/// </remarks>
public class ListOptions
{
	/// <summary>
	/// Only return active (<see langword="true"/>) or inactive records (<c>is_active</c>). Not every endpoint supports it
	/// (time entries and project assignments do not), and those ignore it.
	/// </summary>
	[AliasAs("is_active")]
	public bool? IsActive { get; init; }

	/// <summary>
	/// Only return records updated since this date and time (<c>updated_since</c>), sent as ISO 8601 in UTC
	/// (<c>2017-06-26T21:52:18Z</c>). A <see cref="DateTime"/> converts implicitly; give it a <see cref="DateTimeKind"/>
	/// of <see cref="DateTimeKind.Utc"/> or <see cref="DateTimeKind.Local"/> so the instant is unambiguous.
	/// </summary>
	[AliasAs("updated_since")]
	public DateTimeOffset? UpdatedSince { get; init; }

	/// <summary>The pagination cursor (<c>cursor</c>) from the previous response's <see cref="PaginationLinks.NextCursor"/>.</summary>
	[AliasAs("cursor")]
	public string? Cursor { get; init; }

	/// <summary>The page to return (<c>page</c>); Harvest's default is 1. Harvest prefers <see cref="Cursor"/>.</summary>
	[AliasAs("page")]
	public int? Page { get; init; }

	/// <summary>The number of records per page, from 1 to 2000 (<c>per_page</c>); Harvest's default is 2000.</summary>
	[AliasAs("per_page")]
	public int? PerPage { get; init; }
}