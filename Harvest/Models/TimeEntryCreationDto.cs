namespace Harvest.Models;

/// <summary>
/// The body of <c>POST /v2/time_entries</c>. Give <see cref="TimeEntryDtoBase.Hours"/> on an account that tracks time by
/// duration, or <see cref="TimeEntryDtoBase.StartedTime"/> and <see cref="TimeEntryDtoBase.EndedTime"/> on one that
/// tracks start and end times.
/// </summary>
public class TimeEntryCreationDto : TimeEntryDtoBase
{
	/// <summary>The ID of the user to associate with the time entry. Defaults to the currently authenticated user's ID.</summary>
	public long? UserId { get; set; }

	/// <summary>The ID of the project to associate with the time entry.</summary>
	public required long ProjectId { get; set; }

	/// <summary>The ID of the task to associate with the time entry.</summary>
	public required long TaskId { get; set; }

	/// <summary>The date of the time entry.</summary>
	public required DateOnly SpentDate { get; set; }
}