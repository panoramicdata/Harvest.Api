namespace Harvest.Models;

/// <summary>
/// Filtering and paging for <c>GET /v2/estimates</c>. Harvest does not document <see cref="ListOptions.IsActive"/> for
/// estimates.
/// </summary>
public class EstimateListOptions : ListOptions
{
	/// <summary>Only return estimates belonging to the client with this ID (<c>client_id</c>).</summary>
	[AliasAs("client_id")]
	public long? ClientId { get; init; }

	/// <summary>Only return estimates with an <c>issue_date</c> on or after this date (<c>from</c>).</summary>
	[AliasAs("from")]
	public DateOnly? From { get; init; }

	/// <summary>Only return estimates with an <c>issue_date</c> on or before this date (<c>to</c>).</summary>
	[AliasAs("to")]
	public DateOnly? To { get; init; }

	/// <summary>Only return estimates with this state (<c>state</c>): one of the <see cref="EstimateState"/> values.</summary>
	[AliasAs("state")]
	public string? State { get; init; }
}