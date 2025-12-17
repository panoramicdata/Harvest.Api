using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// Time entry given by the Harvest API.
/// </summary>
public class TimeEntryPatchDto : PatchDto
{
	/// <summary>
	/// The ID of the project to associate with the time entry.
	/// </summary>
	[AliasAs("project_id")]
	public long? ProjectId { get; set; }

	/// <summary>
	/// The ID of the task to associate with the time entry.
	/// </summary>
	[AliasAs("task_id")]
	public long? TaskId { get; set; }

	/// <summary>
	/// The time the entry started. Defaults to the current time. Example: "8:00am".
	/// </summary>
	[AliasAs("spent_date")]
	public string? SpentDate { get; set; }

	/// <summary>
	/// The time the entry started. Defaults to the current time. Example: "8:00am".
	/// </summary>
	[AliasAs("started_time")]
	public string? StartedTime { get; set; }

	/// <summary>
	/// The time the entry ended.
	/// </summary>
	[AliasAs("ended_time")]
	public string? EndedTime { get; set; }

	/// <summary>
	/// The current amount of time tracked.
	/// </summary>
	[AliasAs("hours")]
	public decimal? Hours { get; set; }

	/// <summary>
	/// Any notes to be associated with the time entry.
	/// </summary>
	[AliasAs("notes")]
	public string? Notes { get; set; }

	/// <summary>
	/// An object containing the id, group_id, and permalink of the external reference.
	/// </summary>
	[AliasAs("external_reference")]
	public ExternalReference? ExternalReference { get; set; }
}
