namespace Harvest.Models;

/// <summary>Filtering and paging for <c>GET /v2/invoices</c>. Harvest ignores the inherited <see cref="ListOptions.IsActive"/>.</summary>
public class InvoiceListOptions : ListOptions
{
	/// <summary>Only return invoices belonging to the client with this ID (<c>client_id</c>).</summary>
	[AliasAs("client_id")]
	public long? ClientId { get; init; }

	/// <summary>Only return invoices associated with the project with this ID (<c>project_id</c>).</summary>
	[AliasAs("project_id")]
	public long? ProjectId { get; init; }

	/// <summary>Only return invoices with an <c>issue_date</c> on or after this date (<c>from</c>).</summary>
	[AliasAs("from")]
	public DateOnly? From { get; init; }

	/// <summary>Only return invoices with an <c>issue_date</c> on or before this date (<c>to</c>).</summary>
	[AliasAs("to")]
	public DateOnly? To { get; init; }

	/// <summary>
	/// Only return invoices in this state (<c>state</c>): one of the <see cref="InvoiceState"/> values (<c>draft</c>,
	/// <c>open</c>, <c>paid</c> or <c>closed</c>).
	/// </summary>
	[AliasAs("state")]
	public string? State { get; init; }
}