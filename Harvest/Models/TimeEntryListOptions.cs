namespace Harvest.Models;

/// <summary>Filtering and paging for <c>GET /v2/time_entries</c>.</summary>
public class TimeEntryListOptions : ListOptions
{
	/// <summary>Only return time entries belonging to this user (<c>user_id</c>).</summary>
	[AliasAs("user_id")]
	public long? UserId { get; init; }

	/// <summary>Only return time entries belonging to this client (<c>client_id</c>).</summary>
	[AliasAs("client_id")]
	public long? ClientId { get; init; }

	/// <summary>Only return time entries belonging to this project (<c>project_id</c>).</summary>
	[AliasAs("project_id")]
	public long? ProjectId { get; init; }

	/// <summary>Only return time entries belonging to this task (<c>task_id</c>).</summary>
	[AliasAs("task_id")]
	public long? TaskId { get; init; }

	/// <summary>Only return time entries with this external reference ID (<c>external_reference_id</c>).</summary>
	[AliasAs("external_reference_id")]
	public string? ExternalReferenceId { get; init; }

	/// <summary>Only return time entries that have (<see langword="true"/>) or have not been invoiced (<c>is_billed</c>).</summary>
	[AliasAs("is_billed")]
	public bool? IsBilled { get; init; }

	/// <summary>Only return running (<see langword="true"/>) or stopped time entries (<c>is_running</c>).</summary>
	[AliasAs("is_running")]
	public bool? IsRunning { get; init; }

	/// <summary>
	/// Only return time entries with this approval status (<c>approval_status</c>): one of the
	/// <see cref="Models.ApprovalStatus"/> values.
	/// </summary>
	[AliasAs("approval_status")]
	public string? ApprovalStatus { get; init; }

	/// <summary>Only return time entries spent on or after this date (<c>from</c>).</summary>
	[AliasAs("from")]
	public DateOnly? From { get; init; }

	/// <summary>Only return time entries spent on or before this date (<c>to</c>).</summary>
	[AliasAs("to")]
	public DateOnly? To { get; init; }
}