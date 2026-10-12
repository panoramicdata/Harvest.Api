namespace Harvest.Models;

/// <summary>Filtering and paging for <c>GET /v2/pto/requests</c>. <see cref="ListOptions.IsActive"/> is not supported and ignored.</summary>
public class PtoRequestListOptions : ListOptions
{
	/// <summary>Filter requests by user ID (<c>user_id</c>). Non-administrators can only pass their own user ID.</summary>
	[AliasAs("user_id")]
	public long? UserId { get; init; }

	/// <summary>
	/// Filter requests by status (<c>status</c>): <c>pending</c>, <c>approved</c>, <c>declined</c>,
	/// <c>changes_requested</c> or <c>cancelled</c> (see <see cref="PtoRequestStatus"/>).
	/// </summary>
	[AliasAs("status")]
	public string? Status { get; init; }

	/// <summary>Filter requests by time off policy ID (<c>pto_type_id</c>).</summary>
	[AliasAs("pto_type_id")]
	public long? PtoTypeId { get; init; }

	/// <summary>Only return requests overlapping with or on/after this date (<c>from</c>).</summary>
	[AliasAs("from")]
	public DateOnly? From { get; init; }

	/// <summary>Only return requests overlapping with or on/before this date (<c>to</c>).</summary>
	[AliasAs("to")]
	public DateOnly? To { get; init; }
}