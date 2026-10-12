namespace Harvest.Models;

/// <summary>The body of <c>PATCH /v2/time_entries/{id}</c>.</summary>
public class TimeEntryPatchDto : TimeEntryDtoBase
{
	/// <summary>The ID of the project to associate with the time entry.</summary>
	public long? ProjectId { get; set; }

	/// <summary>The ID of the task to associate with the time entry.</summary>
	public long? TaskId { get; set; }

	/// <summary>The date of the time entry.</summary>
	public DateOnly? SpentDate { get; set; }
}