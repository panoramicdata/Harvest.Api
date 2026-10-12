namespace Harvest.Models;

/// <summary>
/// Filtering and paging for <c>GET /v2/expenses</c>. Harvest does not document <see cref="ListOptions.IsActive"/> for
/// this endpoint.
/// </summary>
public class ExpenseListOptions : ListOptions
{
	/// <summary>Only return expenses belonging to this user (<c>user_id</c>).</summary>
	[AliasAs("user_id")]
	public long? UserId { get; init; }

	/// <summary>Only return expenses belonging to this client (<c>client_id</c>).</summary>
	[AliasAs("client_id")]
	public long? ClientId { get; init; }

	/// <summary>Only return expenses belonging to this project (<c>project_id</c>).</summary>
	[AliasAs("project_id")]
	public long? ProjectId { get; init; }

	/// <summary>Only return expenses that have (<see langword="true"/>) or have not been invoiced (<c>is_billed</c>).</summary>
	[AliasAs("is_billed")]
	public bool? IsBilled { get; init; }

	/// <summary>
	/// Only return expenses with this approval status (<c>approval_status</c>): one of the
	/// <see cref="Models.ApprovalStatus"/> values.
	/// </summary>
	[AliasAs("approval_status")]
	public string? ApprovalStatus { get; init; }

	/// <summary>Only return expenses with a spent_date on or after this date (<c>from</c>).</summary>
	[AliasAs("from")]
	public DateOnly? From { get; init; }

	/// <summary>Only return expenses with a spent_date on or before this date (<c>to</c>).</summary>
	[AliasAs("to")]
	public DateOnly? To { get; init; }
}