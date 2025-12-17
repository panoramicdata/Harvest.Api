using Harvest.Dtos;

namespace Harvest.Models;

/// <summary>
/// Time entry given by the Harvest API.
/// </summary>
public class TimeEntryCreationDto : CreationDto
{
	/// <summary>
	/// The ID of the user to associate with the time entry. Defaults to the currently authenticated user’s ID.
	/// </summary>
	[AliasAs("user_id")]
	public long? UserId { get; set; }

	/// <summary>
	/// The ID of the project to associate with the time entry.
	/// </summary>
	[AliasAs("project_id")]
	public long ProjectId { get; set; }

	/// <summary>
	/// The ID of the task to associate with the time entry.
	/// </summary>
	[AliasAs("task_id")]
	public long TaskId { get; set; }

	[AliasAs("spent_date")]
	public string? SpentDate { get; set; }

	[AliasAs("started_time")]
	public string? StartedTime { get; set; }

	[AliasAs("ended_time")]
	public string? EndedTime { get; set; }      /// <summary>
												/// The current amount of time tracked. Defaults to 0.0.
												/// </summary>
	[AliasAs("hours")]
	public decimal? Hours { get; set; }

	/// <summary>
	/// Any notes to be associated with the time entry.
	/// </summary>
	[AliasAs("notes")]
	public string? Notes { get; set; }      /// <summary>
											/// An object containing the id, group_id, and permalink of the external reference.
											/// </summary>
	[AliasAs("external_reference")]
	public ExternalReferenceCreationDto? ExternalReference { get; set; }
}
